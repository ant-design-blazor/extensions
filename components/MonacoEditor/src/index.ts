import { onReady } from "../../scripts/util";

import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
// import "monaco-editor/esm/vs/basic-languages/javascript/javascript.contribution";
// import 'monaco-editor/esm/vs/editor/contrib/find/findController';


const getMonaco = () => {
    return (window as any).monaco as any;
}

const defaultOptions: any = {
    language: "javascript",
    theme: "vs",
    automaticLayout: true,
    minimap: {
        // 关闭代码缩略图
        enabled: false, // 是否启用预览图
    },
};

export const init = (dotnetObj: any, id: string, options: any) => {
    const editor = new CodeEditor(id, options);

    onReady(() => {
        editor.initMonaco();
        dotnetObj.invokeMethodAsync("Ready")
    });

    return editor;
};

export class CodeEditor {
    private _id: string;
    private _editor: monaco.editor.IStandaloneCodeEditor | null = null;
    private editorOptions: any;

    /**
     *
     */
    constructor(id: string, options: any) {
        const othersOptions = options.othersOptions ?? {};
        othersOptions.language = options.language;
        othersOptions.theme = options.theme;
        this.editorOptions = othersOptions;
        if (!id) {
            throw "codeEditor id is null";
        }
        //console.log(options);
        this._id = id;
    }

    initMonaco() {
        const ele = document.getElementById(this._id);
        if (!ele) {
            throw "element not found for " + this._id;
        }
        const mergeOption = Object.assign(defaultOptions, this.editorOptions);
        this._editor = getMonaco().editor.create(ele, mergeOption);
    }

    setValue(val: string) {
        this._editor!.getModel()?.setValue(val);
    }

    getValue() {
        return this._editor!.getModel()?.getValue();
    }

    async setLanguage(language: string) {
        console.log("language", language);
        const model = this._editor!.getModel();
        if (model) {
            (getMonaco()).editor.setModelLanguage(model, language);
        }
    }

    dispose() {
        this._editor?.dispose();
    }
}
