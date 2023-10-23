const delObs = new WeakMap<Node, () => void>();

export const add = (a: number, b: number) => {
    return a + b;
}