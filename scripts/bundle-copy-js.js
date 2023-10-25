const { getAllComponentsName } = require("./common")
const {  bundleCopyTs } = require('./component-copy-base')


getAllComponentsName().forEach(function (componentName, index) {
  console.log("bundle copy ts", componentName);
  bundleCopyTs(componentName)
})
