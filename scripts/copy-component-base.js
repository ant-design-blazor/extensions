const path = require('path')
const fs = require('fs-extra')
const glob = require('glob')

const { getAllComponentsName, projRootDir  }  = require("./common")



const copyOneComponentTs = (componentName) =>{
  copyOneComponent(componentName, "_scripts", "ts");
}

const copyOneComponentScss = (componentName, theme="default") =>{
  copyOneComponent(componentName, "_styles", "scss");
}

const copyOneComponent = (componentName, commonDir, type, theme="default")=> {
  const tasks = [];

  const componentDir = path.join(projRootDir, componentName);
  const outputBaseDir = `${componentDir}/wwwroot/src`;

  // index.ts or default.scss file content
  let tsFileStr = `import AntDesign from "./ts/main";\nimport * as common from "./ts/common";\n\n`
  let tsInterop = "AntDesign.common = common;\n";
  let scssFileStr = `@import './scss/theme-default.scss';\n@import './scss/variables.scss';\n\n`

  // copy common scss and ts
  copyCommonToComponentWwwroot(tasks, outputBaseDir, commonDir, type)


  // copy all component scss and ts
  copyToComponentWwwroot(tasks, outputBaseDir, componentName, type)
  if(type == "ts"){
    tsFileStr += `import * as ${componentName} from './ts/${componentName}';\n`;
    tsInterop += `AntDesign.interop.${componentName} = ${componentName};\n`;
  }else if(type == "scss"){
    scssFileStr += `@import './scss/${componentName}/index.scss';\n`
  }

  // write index.ts or default scss
  if(type == "ts"){
    createIndexFile(tasks, outputBaseDir,  tsFileStr += `\n\n` + tsInterop, "index.ts")
  }else if(type == "scss"){
    createIndexFile(tasks, outputBaseDir,  scssFileStr, "default.scss")
  }
}

function copyToComponentWwwroot(tasks, outputBaseDir, componentName, type) {
  // read dir
  const componentDir = path.join(projRootDir, componentName);
  const componentDirAbs = path.resolve(__dirname, `../${componentDir}`);
  fs.ensureDirSync(componentDirAbs)

  // output dir
  const outputDir = `${outputBaseDir}/${type}/${componentName}` 
  const outputDirAbs = path.resolve(__dirname, `../${outputDir}`);
  fs.ensureDirSync(outputDirAbs)

  // copy
  const componentFiles = glob.globSync(`${componentDirAbs}/*.${type}`)
  componentFiles.map((cs) => {
    // cs: code source, example: ${projRootDir}\{ComponentName}\index.ts
    // __dirname: /repo/scripts
    if(cs.indexOf("wwwroot") > -1){
      return
    }
    const srcFile = path.resolve(__dirname, `../${cs}`);

    const fileName = path.basename(cs)
    const targetFile = path.join(outputDirAbs, fileName);

    tasks.push(
      fs.copy(
        srcFile,
        targetFile
      )
      .catch((error) => {
        console.log(error);
      })
    )

    if (!fileName.endsWith("index.ts") || !fileName.endsWith("index.scss")) {
      return;
    }
  });
}


function copyCommonToComponentWwwroot(tasks, outputDir, dirName, type) {
  // 将 _scripts 文件夹下的文件拷贝到 src
  const commonScriptDir = path.resolve(__dirname, `../${projRootDir}/${dirName}`);
  if (fs.existsSync(commonScriptDir)) {
    tasks.push(
      fs.copy(
        commonScriptDir,
        path.resolve(__dirname, `../${outputDir}/${type}`)
      )
      .catch((error) => {
        console.log(error);
      })
    )
  }
}

function createIndexFile(tasks, outputDir, fileContent, fileNameWithExt) {
  Promise.all(tasks).then((res) => {
    fs.outputFile(
      path.resolve(__dirname, `../${outputDir}/${fileNameWithExt}`),
      fileContent,
      'utf8',
      (error) => {
        // logger.success(`文件写入成功`);
      }
    );
  })
}



module.exports = { copyOneComponentTs, copyOneComponentScss, copyOneComponent }
