// import { build } from 'vite';
const {build} = require('vite')
const {getAllComponentsEntryFiles, createScssConfig} = require('./component-build-base')


const allEntryFiles = getAllComponentsEntryFiles("default.scss")

// build
allEntryFiles.forEach(async (libItem) => {
  console.log("build ", libItem.entry);
  const viteConfig = createScssConfig(libItem);
  console.log("build viteConfig", viteConfig);
  await build(viteConfig);
});
