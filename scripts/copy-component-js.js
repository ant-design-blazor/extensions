const { getAllComponentsName }  = require("./common")
const {copyOneComponentTs} = require('./copy-component-base')

getAllComponentsName().forEach(function (componentName, index) {
  console.log("copy scss", componentName);
  copyOneComponentTs(componentName)
})
