const path = require('path')
const fs = require('fs-extra')
const glob = require('glob')

const projDirName = "AntDesign.Components"

const componentsJs = glob.globSync(`./src/${projDirName}/**/*.ts`)
const outputDir = `./src/${projDirName}/wwwroot/src/ts`

let tasks = []
let fileStr = `import AntDesign from "./main";\nimport * as common from "./common";\n\n`
let interop = "AntDesign.common = common;\n";

fs.ensureDirSync(outputDir)

componentsJs.map((cs) => {
  cs = cs.replaceAll('\\', '/')
  // common
  if (cs.indexOf(`src/${projDirName}/_scripts`) > -1 || cs.indexOf("wwwroot") > -1) {
    return
  }

  // cs: code source, example: src\${projDirName}\Button\index.ts
  const tempFilePath = `${cs.replace(`src/${projDirName}/`, 'components/')}`;
  const srcFile = path.resolve(__dirname, `../${cs}`);
  // copy file path: src\${projDirName}\Button\index.ts --> src\${projDirName}\wwwroot/src\components\Button\index.ts
  const targetFile = path.resolve(__dirname, `../${outputDir}`, tempFilePath);
  tasks.push(
    fs
      .copy(
        srcFile,
        targetFile
      )
      .catch((error) => { })
  )

  console.log(tempFilePath);
  if (!tempFilePath.endsWith("index.ts")) {
    return;
  }

  let pathInfo = tempFilePath.split('/');
  let componentName = pathInfo[pathInfo.length - 2];
  fileStr += `import * as ${componentName} from './${tempFilePath.replace(".ts", "")}';\n`;
  interop += `AntDesign.interop.${componentName} = ${componentName};\n`;
})


// 将 _scripts 文件夹下的文件拷贝到temp目录下
const commonScriptDir = `../src/${projDirName}/_scripts`;
if(fs.existsSync(commonScriptDir.substring(1))){
  tasks.push(
    fs.copy(
      path.resolve(__dirname, commonScriptDir),
      path.resolve(__dirname, `../${outputDir}`)
    )
  )
}


fileStr += `\n\n` + interop;
Promise.all(tasks).then((res) => {
  fs.outputFile(
    path.resolve(__dirname, `../${outputDir}/index.ts`),
    fileStr,
    'utf8',
    (error) => {
      // logger.success(`文件写入成功`);
    }
  )
})
