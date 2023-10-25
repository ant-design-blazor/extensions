import { defineConfig } from "vite";

const path = require("path");
const atImport = require("postcss-import");
const { banner, projRootDirName, projRootDir }  = require("./scripts/common")


// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  var isProd = mode === "production";
  console.log("isProd", isProd)
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
      outDir: `./dist`,
      rollupOptions: {
        input: {
          entry:{
            index: `./${projRootDir}/bundle/src/index.ts`,
            default:`./${projRootDir}/bundle/src/default.scss`,
          }
        },
        output: {
          banner,
          assetFileNames: `[name].[ext]`,
          entryFileNames: "[name].js",
          chunkFileNames: "[name]-[hash].js",
        },
      },
    },
  };
});


