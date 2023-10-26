
if(!(window as any).AntDesign){
    (window as any).AntDesign = {};
}

(window as any).AntDesign.ext = {};

(window as any).process = {
    env: {
        NODE_ENV: "product"
    }
}