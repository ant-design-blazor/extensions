const { getAllComponentsName }  = require("./common")
const {copyOneComponentScss} = require('./copy-component-base')


getAllComponentsName().forEach(function (componentName, index) {
  console.log("copy scss", componentName);
  copyOneComponentScss(componentName, theme="default")
})
