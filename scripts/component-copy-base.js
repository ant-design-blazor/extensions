const path = require('path')
const fs = require('fs-extra')
const glob = require('glob')

const { globalMount, projRootDir } = require("./common")


const bundleCopyTs = (componentName) => {
  const outputBaseDir = path.join(projRootDir, "bundle/src");
  copyOneComponent(componentName, "_scripts", outputBaseDir, "ts");
}

const bundleCopyScss = (componentName, theme = "default") => {
  const outputBaseDir = path.join(projRootDir, "bundle/src");
  copyOneComponent(componentName, "_styles", outputBaseDir, "scss", theme);
}

const componentCopyTs = (componentName) => {
  const componentDir = path.join(projRootDir, componentName);
  const outputBaseDir = `${componentDir}/wwwroot/src`;
  copyOneComponent(componentName, "_scripts", outputBaseDir, "ts");
}

const componentCopyScss = (componentName, theme = "default") => {
  const componentDir = path.join(projRootDir, componentName);
  const outputBaseDir = `${componentDir}/wwwroot/src`;

  copyOneComponent(componentName, "_styles", outputBaseDir, "scss", theme);
}

const copyOneComponent = (componentName, commonDir, outputBaseDir, type, theme = "default") => {
  const tasks = [];

  // index.ts or default.scss file content
  let tsInterop = `(window as any).${globalMount}.common = common;\n`;
  let tsFileStr = `import "./scripts/main"; \nimport * as common from "./scripts/common";\n\n`
  let scssFileStr = `@import './styles/theme-default.scss';\n@import './styles/variables.scss';\n\n`

  // copy common scss and ts
  copyCommonToComponentWwwroot(tasks, outputBaseDir, commonDir, type)


  // copy all component scss and ts
  copyToComponentWwwroot(tasks, outputBaseDir, componentName, type)
  if (type == "ts") {
    tsFileStr += `import * as ${componentName} from './ts/${componentName}';\n`;
    tsInterop += `(window as any).${globalMount}.${componentName} = ${componentName};\n`;
  } else if (type == "scss") {
    scssFileStr += `@import './scss/${componentName}/index.scss';\n`
  }

  // write index.ts or default scss
  if (type == "ts") {
    createIndexFile(tasks, outputBaseDir, tsFileStr += `\n\n` + tsInterop, "index.ts")
  } else if (type == "scss") {
    createIndexFile(tasks, outputBaseDir, scssFileStr, "default.scss")
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
    if (cs.indexOf("wwwroot") > -1) {
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
  let commonDir = "";
  if (type.toLowerCase() == "ts") {
    commonDir = "scripts"
  } else if (type.toLowerCase() == "scss") {
    commonDir = "styles"
  } else {
    return
  }
  // 将 _scripts 文件夹下的文件拷贝到 src
  const commonScriptDir = path.resolve(__dirname, `../${projRootDir}/${dirName}`);
  if (fs.existsSync(commonScriptDir)) {
    tasks.push(
      fs.copy(
        commonScriptDir,
        path.resolve(__dirname, `../${outputDir}/${commonDir}`)
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



module.exports = {
  bundleCopyTs,
  bundleCopyScss,
  componentCopyTs,
  componentCopyScss,
  copyOneComponent
}
