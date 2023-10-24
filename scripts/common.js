

const path = require('path')
const fs = require('fs')

const projRootDirName = "AntDesign.Components"
const projRootDir = `src/${projRootDirName}`;
const ignoreDir = [ "_scripts", "_styles", "wwwroot"]

const getAllComponentsName = ()=>{
  const files = fs.readdirSync(projRootDir);
  return files.filter(componentName =>{
    let componentDir = path.join(projRootDir, componentName);
    if (!fs.statSync(componentDir).isDirectory()) {
      return false
    }
    if(!componentName || ignoreDir.indexOf(componentName) > -1){
      return false
    }
    return true
  })
}

console.log("getAllComponentsName", getAllComponentsName());

module.exports = { getAllComponentsName, projRootDirName, projRootDir  }
