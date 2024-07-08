import { defineConfig } from "vite";

const path = require("path");
const atImport = require("postcss-import");
const { banner, projRootDirName, projRootDir } = require("./scripts/common")

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
            `./${projRootDir}/_bundle/src`
          ),
        },
      ],
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
      minify: isProd,
      sourcemap: !isProd,
      emptyOutDir: false,
      outDir: `./dist`,
      rollupOptions: {
        input: {
          default: `./${projRootDir}/_bundle/src/default.scss`,
        },
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