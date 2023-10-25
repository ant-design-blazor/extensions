import { defineConfig } from "vite";

const path = require("path");
const atImport = require("postcss-import");
const { banner, projRootDir } = require("./scripts/common");

// https://vitejs.dev/config/
export default defineConfig({
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
    emptyOutDir: false,
    outDir: `./dist`,
    rollupOptions: {
      input: `./${projRootDir}/bundle/src/default.scss`,
      output: {
        banner,
        assetFileNames: `[name].[ext]`,
        entryFileNames: "[name].js",
        chunkFileNames: "[name]-[hash].js",
      },
    },
  },
});
