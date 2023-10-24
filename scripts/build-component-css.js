// import { build } from 'vite';
const {build} = require('vite')
const {getAllComponentsEntryFiles, createScssConfig} = require('./build-component-base')


const allEntryFiles = getAllComponentsEntryFiles("default.scss")

// build
allEntryFiles.forEach(async (libItem) => {
  console.log("build ", libItem.entry);
  const viteConfig = createScssConfig(libItem);
  await build(viteConfig);
});
