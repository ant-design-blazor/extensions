
if (!(window as any).antd){
    (window as any).antd = {};
}

(window as any).antd.ext = {};

(window as any).process = {
    env: {
        NODE_ENV: "product"
    }
}