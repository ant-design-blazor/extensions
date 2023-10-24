import { defineConfig } from "vite";

const path = require("path");
const config = require("./package.json");

const projDirName = "AntDesign.Components"
const libName = "ant-design-blazor"

const banner = `/*!
* ${config.name} v${config.version} ${new Date()}
* (c) 2023 @${libName}
* Released under the MIT License.
*/`;

// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
    var isProd = mode === 'production';

    return {
        resolve: {
          alias: [{ find: "@", replacement: path.resolve(__dirname, "./src") }],
        },
        build: {
          minify: isProd,
          sourcemap: !isProd,
          emptyOutDir: false,
          outDir: `./src/${projDirName}/wwwroot/dist`,
          rollupOptions: {
            output: {
              banner,
              // 入口文件 input 配置所指向的文件包名 默认值："[name].js"
              entryFileNames: (fileInfo) => {
                console.log("entryFileNames", fileInfo.facadeModuleId);
                return "[name].min.js";
              },
            },
          },
          lib: {
            entry: `./src/${projDirName}/wwwroot/src/ts/index.ts`,
            name: libName,
            fileName: libName,
            formats: ["es"],
          },
        },
      }
}
);