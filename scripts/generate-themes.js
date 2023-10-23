const path = require('path')
const fs = require('fs-extra')
const glob = require('glob')

const projDirName = "AntDesign.Components"
const componentsScss = glob.globSync(`./src/${projDirName}/**/*.scss`)
const outputDir = `./src/${projDirName}/wwwroot/src/scss`

let tasks = []


let fileStr = `@import '../theme-default.scss';\n@import '../variables.scss';\n\n`

componentsScss.map((cs) => {
  cs = cs.replaceAll('\\', '/')
  if (cs.indexOf(`src/${projDirName}/_styles`) > -1 || cs.indexOf("wwwroot") > -1) {
    return
  }

  // cs: code source, example: src\${projDirName}\Button\index.scss
  const tempFilePath = `${cs.replace(`src/${projDirName}/`, 'components/')}`;
  const srcFile = path.resolve(__dirname, `../${cs}`);
  const targetFile = path.resolve(__dirname, `../${outputDir}`, tempFilePath);

  // console.log(`${cs}, ${srcFile} --> ${targetFile}`);
  tasks.push(
    fs
      .copy(
        srcFile,
        targetFile
      )
      .catch((error) => {})
  )

  fileStr += `@import '../../${tempFilePath}';\n`
})

// 将 _scripts 文件夹下的文件拷贝到temp目录下
const commonScriptDir = `../src/${projDirName}/_styles`;
if(fs.existsSync(commonScriptDir.substring(1))){
  tasks.push(
    fs.copy(
      path.resolve(__dirname, commonScriptDir),
      path.resolve(__dirname, `../${outputDir}/styles`)
    )
  )
}


Promise.all(tasks).then((res) => {
  fs.outputFile(
    path.resolve(__dirname, `../${outputDir}/styles/themes/default.scss`),
    fileStr,
    'utf8',
    (error) => {
      // logger.success(`文件写入成功`);
    }
  )
})