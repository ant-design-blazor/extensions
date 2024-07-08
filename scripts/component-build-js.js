// import { build } from 'vite';
const {build} = require('vite')
const path = require('path')
const fs = require('fs-extra')

const {getAllComponentsName, projRootDir} = require("./common")
const {getAllComponentsEntryFiles, createTsConfig} = require('./component-build-base')


const allEntryFiles = getAllComponentsEntryFiles("index.ts")

// build
allEntryFiles.forEach(async (libItem) => {
  console.log("build ", libItem.entry);
  const viteConfig = createTsConfig(libItem);
  await build(viteConfig);
});

// 写 *.lib.module.js
getAllComponentsName().forEach(function (componentName, index) {
  const outputFile = `../${projRootDir}/${componentName}/src/wwwroot/AntDesign.Extensions.Components.${componentName}.lib.module.js`;
  console.log("outputFile:", outputFile);
  const fileContent = createStartupJs(`AntDesign.Extensions.Components.${componentName}`);
  fs.outputFile(
    path.resolve(__dirname, outputFile),
    fileContent,
    'utf8',
    (error) => {
      console.log(`${componentName}.lib.module.js 写入成功`);
    }
  );
})


function createStartupJs(componentName) {
  return `var beforeStartCalled = false;
  var afterStartedCalled = false;
  
  export function beforeWebStart() {
    loadScriptAndStyle();
  }
  
  export function beforeStart(options, extensions) {
    loadScriptAndStyle();
  }
  
  function loadScriptAndStyle() {
    if (beforeStartCalled) {
        return;
    }
  
    beforeStartCalled = true;
  
    if (!document.querySelector('[src$="_content/${componentName}/dist/index.js"]') && !document.querySelector('[no-antblazor-js]')) {
        var customScript = document.createElement('script');
        customScript.setAttribute('src', '_content/${componentName}/dist/index.js');
        const jsmark = document.querySelector('[antblazor-js]') || document.querySelector('script');
  
        if (jsmark) {
            jsmark.before(customScript);
        } else {
            document.body.appendChild(customScript);
        }
    }
  
    if (!document.querySelector('[href*="_content/${componentName}/dist/default.css"]') && !document.querySelector('[no-antblazor-css]')) {
        var customStyle = document.createElement('link');
        customStyle.setAttribute('href', '_content/${componentName}/dist/default.css');
        customStyle.setAttribute('rel', 'stylesheet');
  
        const cssMark = document.querySelector('[antblazor-css]') || document.querySelector('link');
        if (cssMark) {
            cssMark.before(customStyle);
        } else {
            document.head.appendChild(customStyle);
        }
    }
  }`;
}
