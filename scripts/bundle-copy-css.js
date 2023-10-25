const { getAllComponentsName } = require("./common")
const {  bundleCopyScss } = require('./component-copy-base')


getAllComponentsName().forEach(function (componentName, index) {
  console.log("bundle copy scss", componentName);
  bundleCopyScss(componentName, theme="default")
})
