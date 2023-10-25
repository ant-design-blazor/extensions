const { getAllComponentsName }  = require("./common")
const {componentCopyTs} = require('./component-copy-base')

getAllComponentsName().forEach(function (componentName, index) {
  console.log("copy scss", componentName);
  componentCopyTs(componentName)
})
