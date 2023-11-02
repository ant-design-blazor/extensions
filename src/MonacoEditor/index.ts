import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
// import "monaco-editor/esm/vs/basic-languages/javascript/javascript.contribution";
// import 'monaco-editor/esm/vs/editor/contrib/find/findController';

const getMonaco = () => (window as any).monaco as any;

const defaultOptions: any = {
  language: "javascript",
  theme: "vs",
  automaticLayout: true,
  minimap: {
    // 关闭代码缩略图
    enabled: false, // 是否启用预览图
  },
};

export const init = (id: string, options: any) => {
  return new CodeEditor(id, options);
};

export class CodeEditor {
  private _id: string;
  private _editor: monaco.editor.IStandaloneCodeEditor;
  /**
   *
   */
  constructor(id: string, options: any) {
    var othersOptions = options.othersOptions ?? {};
    othersOptions.language = options.language;
    othersOptions.theme = options.theme;
    const mergeOption = Object.assign(defaultOptions, othersOptions);
    if (!id) {
      throw "codeEditor id is null";
    }
    const ele = document.getElementById(id);
    if (!ele) {
      throw "element not found for " + id;
    }
    this._id = id;
    this._editor = getMonaco().editor.create(ele, mergeOption);
  }

  setValue(val: string) {
    this._editor.getModel()?.setValue(val);
  }

  getValue() {
    return this._editor.getModel()?.getValue();
  }

  setLanguage(language: string) {
    const model = this._editor.getModel();
    if (model) {
      getMonaco().editor.setModelLanguage(model, language);
    }
  }

  dispose() {
    this._editor.dispose();
  }
}
