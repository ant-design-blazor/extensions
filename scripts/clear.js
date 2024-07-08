
const path = require('path')
const fs = require('fs')

const { getAllComponentsName, projRootDir } = require("./common")

// 删除 bundle 中的 src 和 dist
rmDir(path.resolve(__dirname, `../components/_bundle/src`));
rmDir(path.resolve(__dirname, `../dist`));

// 删除模块中的 wwwroot 下的 src 和 dist
getAllComponentsName().forEach(function (componentName, index) {
  console.log("clear component", componentName);
  const componentDir = path.join(projRootDir, componentName);
  rmDir(path.resolve(__dirname, `../${componentDir}/src/wwwroot/src`));
  rmDir(path.resolve(__dirname, `../${componentDir}/src/wwwroot/dist`));
})

getAllComponentsName().forEach(function (componentName, index) {
  const outputFile = `../${projRootDir}/${componentName}/src/wwwroot/AntDesign.Extensions.Components.${componentName}.lib.module.js`;
  const absPath = path.resolve(__dirname, outputFile)
  fs.unlinkSync(absPath)
})


function rmDir(componentDirAbs) {
  console.log("componentDir", componentDirAbs);
  if(fs.existsSync(componentDirAbs)){
    emptyDir(componentDirAbs);
    rmEmptyDir(componentDirAbs);
  }
}

//删除所有的文件(将所有文件夹置空)
function emptyDir(filePath) {
  const files = fs.readdirSync(filePath)//读取该文件夹
  files.forEach((file) => {
    const nextFilePath = `${filePath}/${file}`
    const states = fs.statSync(nextFilePath)
    if (states.isDirectory()) {
      emptyDir(nextFilePath)
    } else {
      fs.unlinkSync(nextFilePath)
      console.log(`删除文件 ${nextFilePath} 成功`)
    }
  })
}

//删除所有的空文件夹
function rmEmptyDir(filePath) {
  const files = fs.readdirSync(filePath)
  if (files.length === 0) {
    fs.rmdirSync(filePath)
    console.log(`删除空文件夹 ${filePath} 成功`)
  } else {
    let tempFiles = 0
    files.forEach((file) => {
      tempFiles++
      const nextFilePath = `${filePath}/${file}`
      rmEmptyDir(nextFilePath)
    })
    //删除母文件夹下的所有字空文件夹后，将母文件夹也删除
    if (tempFiles === files.length) {
      fs.rmdirSync(filePath)
      console.log(`删除空文件夹 ${filePath} 成功`)
    }
  }
}