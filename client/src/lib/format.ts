export function formatPrice(value: number): string {
    if (value >= 1_000_000) {
        const millions = value / 1_000_000;
        return `${millions % 1 === 0 ? millions : millions.toFixed(1)}M`;
    }
    return `${Math.round(value / 1000)}k`;
}

export function formatPosition(position: number | null, active: boolean): string {
    return position === null ? "" : `${position}${active ? "*" : ""}`;
}
