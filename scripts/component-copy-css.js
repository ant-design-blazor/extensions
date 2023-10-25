const { getAllComponentsName }  = require("./common")
const {componentCopyScss} = require('./component-copy-base')


getAllComponentsName().forEach(function (componentName, index) {
  console.log("copy scss", componentName);
  componentCopyScss(componentName, theme="default")
})
