// import { build } from 'vite';
const {build} = require('vite')
const {getAllComponentsEntryFiles, createTsConfig} = require('./build-component-base')


const allEntryFiles = getAllComponentsEntryFiles("index.ts")

// build
allEntryFiles.forEach(async (libItem) => {
  console.log("build ", libItem.entry);
  const viteConfig = createTsConfig(libItem);
  await build(viteConfig);
});
