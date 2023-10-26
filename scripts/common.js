const path = require('path')
const fs = require('fs')
const config = require('../package.json')

// 全局变量挂载点
const globalMount = "AntDesign.ext"
const projRootDirName = ""
const projRootDir = projRootDirName ? `src/${projRootDirName}`: "src";
const ignoreDir = ["_scripts", "_styles", "wwwroot", "bundle", "Common"]
// console.log("projRootDir", projRootDir);

const getAllComponentsName = () => {
  const files = fs.readdirSync(projRootDir);
  return files.filter(componentName => {
    let componentDir = path.join(projRootDir, componentName);
    if (!fs.statSync(componentDir).isDirectory()) {
      return false
    }
    if (!componentName || ignoreDir.indexOf(componentName) > -1) {
      return false
    }
    return true
  })
}

// console.log("getAllComponentsName", getAllComponentsName());

const org = "ant-design-blazor"

const banner = `/*!
* ${config.name} v${config.version} ${new Date()}
* (c) 2023 @${org}
* Released under the MIT License.
*/`;

module.exports = {  globalMount, getAllComponentsName, projRootDirName, projRootDir, banner }
