import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from "recharts";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { useEffect, useMemo, useRef, useState } from "react";
import axios from "../../api/client";
import { colors } from "./ChartsHelper";
import SmallSwitch from "../../components/shared/SmallSwitch";
import Select from "../../components/Select";
import type { SelectOption } from "../../components/Select";

interface ChartData {
    data: any[];
    usernames: string[];
}

const ALLE_RACES_OPTION: SelectOption<number> = { displayValue: "   ", value: 0 };

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
    const [startRaceId, setStartRaceId] = useState<number>(0);
    const [endRaceId, setEndRaceId] = useState<number>(0);
    const [raceOptions, setRaceOptions] = useState<SelectOption<number>[]>([ALLE_RACES_OPTION]);
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });
    const [toggles, setToggles] = useState<{ username: string; showUser: boolean }[]>([]);
    const prevBudgetParticipation = useRef(budgetParticipation);

    useEffect(() => {
        axios.get(`/api/Charts/racePositieScoreRaceOptions`).then((res) => {
            setRaceOptions([ALLE_RACES_OPTION, ...res.data]);
        });
    }, []);

    // raceOptions is ordered newest to oldest (after the "   " entry at index 0), so the
    // end race can only be the selected start race or a more recent one (a lower index).
    const endRaceOptions = useMemo(() => {
        if (startRaceId === 0) return raceOptions;
        const startIndex = raceOptions.findIndex((o) => o.value === startRaceId);
        if (startIndex === -1) return raceOptions;
        return [ALLE_RACES_OPTION, ...raceOptions.slice(1, startIndex + 1)];
    }, [raceOptions, startRaceId]);

    useEffect(() => {
        if (!endRaceOptions.some((o) => o.value === endRaceId)) {
            setEndRaceId(0);
        }
    }, [endRaceOptions, endRaceId]);

    useEffect(() => {
        const budgetParticipationChanged = prevBudgetParticipation.current !== budgetParticipation;
        prevBudgetParticipation.current = budgetParticipation;

        axios
            .get(`/api/Charts/stagenummerScoreVerloop`, {
                params: { budgetParticipation, genormaliseerd, totaalScore, startRaceId, endRaceId },
            })
            .then((res) => {
                setChartData({ data: res.data.data, usernames: res.data.users });
                setToggles((prev) =>
                    res.data.users.map((username: string) => {
                        const existing = !budgetParticipationChanged && prev.find((t) => t.username === username);
                        return { username, showUser: existing ? existing.showUser : true };
                    })
                );
            })
            .catch((error) => { });
    }, [budgetParticipation, genormaliseerd, totaalScore, startRaceId, endRaceId]);

    const displayedData = useMemo(
        () => applyMovingAverage(chartdata.data, chartdata.usernames, windowSize),
        [chartdata, windowSize]
    );

    const toggleUser = (index: number): void => {
        setToggles((prev) => prev.map((t, i) => (i === index ? { ...t, showUser: !t.showUser } : t)));
    };

    const toggleAll = (): void => {
        const newValue = toggles.some((t) => !t.showUser);
        setToggles((prev) => prev.map((t) => ({ ...t, showUser: newValue })));
    };

    return (
        <div className="chart-frame">
            <div
                style={{
                    display: "flex",
                    flexWrap: "wrap",
                    alignItems: "flex-end",
                    gap: "8px",
                    marginBottom: "10px",
                }}
            >
                <label
                    style={{ display: "flex", alignItems: "center", gap: "4px", color: "white", cursor: "pointer" }}
                >
                    Genormaliseerd (t.o.v. eigen gemiddelde)
                    <input
                        type="checkbox"
                        checked={genormaliseerd}
                        onChange={() => setGenormaliseerd(!genormaliseerd)}
                    />
                </label>
                <label
                    style={{ display: "flex", alignItems: "center", gap: "4px", color: "white", cursor: "pointer" }}
                >
                    Totaalscore (i.p.v. etappescore)
                    <input type="checkbox" checked={totaalScore} onChange={() => setTotaalScore(!totaalScore)} />
                </label>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", color: "white", fontSize: "13px" }}>
                    Start race
                    <Select<number>
                        value={startRaceId}
                        options={raceOptions}
                        onChange={(selectedOption: number) => setStartRaceId(Number(selectedOption))}
                        style={{ minWidth: "85px" }}
                    />
                </label>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", color: "white", fontSize: "13px" }}>
                    Eind race
                    <Select<number>
                        value={endRaceId}
                        options={endRaceOptions}
                        onChange={(selectedOption: number) => setEndRaceId(Number(selectedOption))}
                        style={{ minWidth: "85px" }}
                    />
                </label>
                <label style={{ display: "flex", flexDirection: "column", gap: "4px", color: "white", fontSize: "13px" }}>
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
            <div
                style={{
                    display: "flex",
                    flexWrap: "wrap",
                    alignItems: "center",
                    gap: "6px",
                    marginBottom: "12px",
                }}
            >
                {toggles.map((user, index) => (
                    <SmallSwitch
                        key={index}
                        text={user.username}
                        selected={user.showUser}
                        index={index}
                        toggleUser={() => toggleUser(index)}
                    />
                ))}
                <button onClick={toggleAll}>Toggle alle</button>
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
                {chartdata.usernames
                    .map((username, index) => ({ username, index }))
                    .filter((_, index) => toggles.at(index)?.showUser)
                    .map((user) => (
                        <Line
                            key={user.index}
                            type="linear"
                            dataKey={user.username}
                            stroke={colors[user.index]}
                            strokeWidth={3}
                            dot={false}
                        />
                    ))}
            </LineChart>
        </div>
    );
};

export default StagenummerScoreChart;

