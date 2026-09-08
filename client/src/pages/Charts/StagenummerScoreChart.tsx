import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from "recharts";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { useEffect, useMemo, useState } from "react";
import axios from "../../api/client";
import { colors } from "./ChartsHelper";

interface ChartData {
    data: any[];
    usernames: string[];
}

const MOVING_AVERAGE_OPTIONS = [1, 2, 3, 5, 8];

// Berekent per gebruiker een voortschrijdend gemiddelde over diens eigen (aanwezige) datapunten,
// op volgorde van stagenummer. Ontbrekende stagenummers voor een gebruiker worden overgeslagen.
// Het "0" startpunt (Name === "") is een puur visueel ankerpunt en telt niet mee.
const applyMovingAverage = (data: any[], usernames: string[], windowSize: number): any[] => {
    if (windowSize <= 1) return data;

    const result = data.map((point) => ({ ...point }));
    usernames.forEach((username) => {
        const indices: number[] = [];
        const values: number[] = [];
        data.forEach((point, index) => {
            if (point.Name !== "" && point[username] !== undefined) {
                indices.push(index);
                values.push(Number(point[username]));
            }
        });
        values.forEach((_, i) => {
            const start = Math.max(0, i - windowSize + 1);
            const window = values.slice(start, i + 1);
            const average = window.reduce((sum, value) => sum + value, 0) / window.length;
            result[indices[i]][username] = Math.round(average * 100) / 100;
        });
    });
    return result;
};

const StagenummerScoreChart = () => {
    document.title = "Score per Stagenummer";
    const budgetParticipation = useBudgetContext();
    const [genormaliseerd, setGenormaliseerd] = useState<boolean>(false);
    const [totaalScore, setTotaalScore] = useState<boolean>(false);
    const [windowSize, setWindowSize] = useState<number>(1);
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });

    useEffect(() => {
        axios
            .get(`/api/Charts/stagenummerScoreVerloop`, {
                params: { budgetParticipation, genormaliseerd, totaalScore },
            })
            .then((res) => {
                setChartData({ data: res.data.data, usernames: res.data.users });
            })
            .catch((error) => { });
    }, [budgetParticipation, genormaliseerd, totaalScore]);

    const displayedData = useMemo(
        () => applyMovingAverage(chartdata.data, chartdata.usernames, windowSize),
        [chartdata, windowSize]
    );

    return (
        <div className="chart-frame">
            <div style={{ marginBottom: "10px", display: "flex", flexWrap: "wrap", gap: "16px" }}>
                <label
                    style={{
                        display: "inline-flex",
                        alignItems: "center",
                        gap: "4px",
                        color: "white",
                        cursor: "pointer",
                    }}
                >
                    Genormaliseerd (t.o.v. eigen gemiddelde)
                    <input
                        type="checkbox"
                        checked={genormaliseerd}
                        onChange={() => setGenormaliseerd(!genormaliseerd)}
                    />
                </label>
                <label
                    style={{
                        display: "inline-flex",
                        alignItems: "center",
                        gap: "4px",
                        color: "white",
                        cursor: "pointer",
                    }}
                >
                    Totaalscore (i.p.v. etappescore)
                    <input
                        type="checkbox"
                        checked={totaalScore}
                        onChange={() => setTotaalScore(!totaalScore)}
                    />
                </label>
                <label
                    style={{
                        display: "inline-flex",
                        alignItems: "center",
                        gap: "4px",
                        color: "white",
                    }}
                >
                    Voortschrijdend gemiddelde
                    <select value={windowSize} onChange={(e) => setWindowSize(Number(e.target.value))}>
                        {MOVING_AVERAGE_OPTIONS.map((option) => (
                            <option key={option} value={option}>
                                {option}
                            </option>
                        ))}
                    </select>
                </label>
            </div>
            <LineChart width={1560} height={600} data={displayedData}>
                <CartesianGrid vertical={false} strokeDasharray="3 3" />
                <XAxis dataKey="Name">
                    <Label value="Etappenummer" position="bottom" dy={-15} />
                </XAxis>
                <YAxis tickCount={10}>
                    <Label
                        value={genormaliseerd ? "Relatief t.o.v. eigen gemiddelde" : "Relatieve Punten"}
                        angle={-90}
                        position="left"
                        offset={-10}
                    />
                </YAxis>
                <Tooltip
                    contentStyle={{ backgroundColor: "#333", border: "none" }}
                    labelStyle={{ color: "#fff" }}
                />
                <Legend verticalAlign="top" wrapperStyle={{ marginTop: -10 }} />
                {chartdata.usernames.map((username, index) => (
                    <Line
                        key={index}
                        type="linear"
                        dataKey={username}
                        stroke={colors[index]}
                        strokeWidth={3}
                        dot={false}
                    />
                ))}
            </LineChart>
        </div>
    );
};

export default StagenummerScoreChart;

