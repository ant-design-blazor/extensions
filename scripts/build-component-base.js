// build.js

// import { build } from 'vite';
const {build} = require('vite')
const path = require('path')
const fs = require('fs')
const atImport = require('postcss-import')
const config = require('../package.json')

const { getAllComponentsName, projRootDir  }  = require("./common")

const libName = "ant-design-blazor"

const banner = `/*!
* ${config.name} v${config.version} ${new Date()}
* (c) 2023 @${libName}
* Released under the MIT License.
*/`;

// gene component info
const getAllComponentsEntryFiles = (entryFileName)=>{
  const libraries = [];

  getAllComponentsName().forEach(function (componentName, index) {
    const entryFile = `./${projRootDir}/${componentName}/wwwroot/src/${entryFileName}`;
  
    libraries.push({
      entry: entryFile,
      name: componentName,
      fileName: componentName,
      outDir: `./${projRootDir}/${componentName}/wwwroot/dist/`
    })
  })
  return libraries;
}

const createScssConfig = (libItem) => {
  return {
    configFile: false,
    resolve: {
        alias: [{ find: '@', replacement: path.resolve(__dirname, `./${projRootDir}/wwwroot/src/scss`) }],
    },
    css: {
        preprocessorOptions: {
            // scss: {
            //     charset: false,
            //     additionalData: `@import "@/variables.scss";`,
            // },
            postcss: {
                plugins: [atImport({ path: path.join(__dirname, 'src`') })],
            },
        },
    },
    plugins: [],
    build: {
      emptyOutDir: false, 
      outDir: libItem.outDir,
      // sourcemap: true,
      // lib: {
      //   entry: libItem.entry,
      //   name: libItem.name,
      //   fileName: libItem.fileName,
      // },
      rollupOptions: {
        // other options
        input: libItem.entry,
        output: {
          banner, 
          assetFileNames: `[name].[ext]`,
          entryFileNames: "[name].js",
          chunkFileNames: "[name]-[hash].js",
          // entryFileNames: "[name].[format].js",
          // chunkFileNames: "[name]-[hash].[format].js",
      },
      },
    },
  }
}

const createTsConfig = libItem => {
  return {
    configFile: false,
      resolve: {
        alias: [{ find: '@', replacement: path.resolve(__dirname, `./${projRootDir}/wwwroot/src/ts`) }],
    },
    plugins: [],
    build: {
      emptyOutDir: false, 
      outDir: libItem.outDir,
      // sourcemap: true,
      // lib: {
      //   entry: libItem.entry,
      //   name: libItem.name,
      //   fileName: libItem.fileName,
      // },
      rollupOptions: {
        input: libItem.entry,
        output: {
          banner, 
          assetFileNames: `[name].[ext]`,
          entryFileNames: "[name].js",
          chunkFileNames: "[name]-[hash].js",
          // entryFileNames: "[name].[format].js",
          // chunkFileNames: "[name]-[hash].[format].js",
      },
      },
    },
  }
}

module.exports = { getAllComponentsEntryFiles, createScssConfig, createTsConfig }
