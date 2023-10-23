import { defineConfig } from "vite";

const path = require("path");
const atImport = require("postcss-import");
const config = require("./package.json");

const projDirName = "AntDesign.Components"
const libName = "ant-design-blazor"

const banner = `/*!
* ${config.name} v${config.version} ${new Date()}
* (c) 2023 @${libName}
* Released under the MIT License.
*/`;

const { resolve } = path;
// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
  var isProd = mode === "production";
  console.log("isProd", isProd)
  return {
    resolve: {
      alias: [
        { find: "@", replacement: resolve(__dirname, `./src/${projDirName}/wwwroot/src/scss`) },
      ],
    },
    esbuild: {
      drop: isProd ? ["console", "debugger"] : [],
    },
    css: {
      preprocessorOptions: {
        scss: {
          charset: false,
          additionalData: `@import "@/styles/variables.scss";`,
        },
        postcss: {
          plugins: [atImport({ path: path.join(__dirname, "src`") })],
        },
      },
    },
  plugins: [],
    build: {
      sourcemap: !isProd,
      emptyOutDir: true,
      minify: true,
      outDir: `./src/${projDirName}/wwwroot/dist`,
      rollupOptions: {
        output: {
          banner,
          assetFileNames: (fileInfo) => {
            // console.log("fileInfo", fileInfo);
            // if (fileInfo.name == "style.css") {
            //   return "index.min.css";
            // }
            return `[name].[ext]`;
          },
          // 入口文件 input 配置所指向的文件包名 默认值："[name].js"
          entryFileNames: (fileInfo) => {
            console.log("entryFileNames", fileInfo.facadeModuleId);

            return "[name].min.js";
          },
        },
      },
      lib: {
        entry: {
          index: `./src/${projDirName}/wwwroot/src/ts/index.ts`,
          default:
            `./src/${projDirName}/wwwroot/src/scss/styles/themes/default.scss`,
        },
        name: libName,
        fileName: libName,
        formats: ["es"],
      },
    },
  };
});


