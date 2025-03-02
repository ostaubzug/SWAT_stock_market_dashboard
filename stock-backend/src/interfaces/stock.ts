export interface Stock {
    symbol: string;
    name: string;
    price: number;
    change: number;
    changePercent: number | string;
}

export default Stock;