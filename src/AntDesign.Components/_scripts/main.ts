const AntDesign: any = {
    interop: {}
};

(window as any).AntDesign = AntDesign;

(window as any).process = {
    env: {
        NODE_ENV: "product"
    }
}

export default AntDesign