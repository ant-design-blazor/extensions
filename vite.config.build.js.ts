import { defineConfig } from "vite";

const path = require("path");
const { banner, projRootDirName, projRootDir }  = require("./scripts/common")

// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
    var isProd = mode === 'production';
    console.log("process.env", process.env);
    return {
        resolve: {
          alias: [
            {
              find: "@",
              replacement: path.resolve(
                __dirname,
                `./${projRootDir}/bundle/src`
              ),
            },
          ],
        },
        build: {
          minify: isProd,
          sourcemap: !isProd,
          emptyOutDir: false,
          outDir: `./dist`,
          rollupOptions: {
            input: `./${projRootDir}/bundle/src/index.ts`,
            output: {
              banner,
              assetFileNames: `[name].[ext]`,
              entryFileNames: "[name].js",
              chunkFileNames: "[name]-[hash].js",
            },
          },
        },
      }
}
);