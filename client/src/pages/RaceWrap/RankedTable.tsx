function RankedTable<T>({ data, getUsername, getValue }: { data: T[], getUsername: (v: T) => string, getValue: (v: T) => number }) {
    const rows = data.map((item, idx) => {
        let style: React.CSSProperties = {};
        if (idx === 0) {
            style = { color: "#fde047", fontSize: "1.5rem" }; //yellow-300
        }
        if (idx === 1) {
            style = { color: "#e2e8f0" }; //slate-200
        }
        if (idx === 2) {
            style = { color: "#fbbf24" }; //amber-400
        }
        return (
            <div style={{ display: "flex", justifyContent: "between" }}>
                <span style={style}>{getUsername(item)}</span>
                <span style={style}>{getValue(item)}</span>
            </div>
        );
    });

    return (
        <div style={{ display: "flex", flexDirection: "column" }}>{rows}</div>
    );
};

export default RankedTable;
