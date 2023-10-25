// build.js

const path = require('path')
const atImport = require('postcss-import')

const { getAllComponentsName, projRootDir, banner }  = require("./common")


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
    css: {
      preprocessorOptions: {
        scss: {
          charset: false,
          additionalData: `@import "./styles/variables.scss";`,
        },
        postcss: {
          plugins: [atImport({ path: path.join(__dirname, "src`") })],
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
