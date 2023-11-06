import { getEle } from "./util";


export const MoveEleTo = (
    ref: HTMLElement | string,
    container: HTMLElement | string
) => {
    const ele = getEle(ref);
    const c = getEle(container);
    if (ele && c) {
        c.append(ele);
    } 
}


let disableIndex = 0;
export const DisableBodyScroll = () => {
    disableIndex++;
    if (disableIndex === 1) {
        document.body.classList.add("overflow-hidden")
    }
}
export const EnableBodyScroll = () => {
    disableIndex--;
    if (disableIndex < 1) {
        document.body.classList.remove("overflow-hidden")
    }
}


export const GetBoundingClientRect = (ele: HTMLElement) => {
    if (ele && ele.getBoundingClientRect) {
        const rect = ele.getBoundingClientRect();
        return {
            width: rect.width,
            height: rect.height,
            top: rect.top,
            right: rect.right,
            bottom: rect.bottom,
            left: rect.left,
            x: rect.x,
            y: rect.y
        };
    }
    return null;
}