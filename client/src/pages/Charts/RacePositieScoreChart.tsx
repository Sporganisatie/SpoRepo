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

const PUNTENTELLING_OPTIONS: SelectOption<string>[] = [
    { displayValue: "Aantal deelnemers - positie", value: "AantalDeelnemersMinPositie" },
    { displayValue: "1 punt per overwinning", value: "EenPuntPerOverwinning" },
    { displayValue: "5 2 1 voor podium", value: "Podium521" },
    { displayValue: "4 2 1 voor podium", value: "Podium421" },
    { displayValue: "3 2 1 voor podium", value: "Podium321" },
];

const RacePositieScoreChart = () => {
    document.title = "Positie Score Verloop";
    const budgetParticipation = useBudgetContext();
    const [bigFour, setBigFour] = useState<boolean>(false);
    const [relative, setRelative] = useState<boolean>(false);
    const [startRaceId, setStartRaceId] = useState<number>(0);
    const [endRaceId, setEndRaceId] = useState<number>(0);
    const [raceOptions, setRaceOptions] = useState<SelectOption<number>[]>([ALLE_RACES_OPTION]);
    const [puntentelling, setPuntentelling] = useState<string>("EenPuntPerOverwinning");
    const [data, setData] = useState<ChartData>({ data: [], usernames: [] });
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
            .get(`/api/Charts/racePositieScoreVerloop`, {
                params: { budgetParticipation, bigFour, relative, startRaceId, endRaceId, puntentelling },
            })
            .then((res) => {
                setData({ data: res.data.data, usernames: res.data.users });
                setToggles((prev) =>
                    res.data.users.map((username: string) => {
                        const existing = !budgetParticipationChanged && prev.find((t) => t.username === username);
                        return { username, showUser: existing ? existing.showUser : true };
                    })
                );
            })
            .catch((error) => { });
    }, [budgetParticipation, bigFour, relative, startRaceId, endRaceId, puntentelling]);

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
                    Big Four
                    <input type="checkbox" checked={bigFour} onChange={() => setBigFour(!bigFour)} />
                </label>
                <label
                    style={{ display: "flex", alignItems: "center", gap: "4px", color: "white", cursor: "pointer" }}
                >
                    Relatief
                    <input type="checkbox" checked={relative} onChange={() => setRelative(!relative)} />
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
                    Puntentelling
                    <Select<string>
                        value={puntentelling}
                        options={PUNTENTELLING_OPTIONS}
                        onChange={(selectedOption: string) => setPuntentelling(selectedOption)}
                    />
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
            <div>
                <LineChart width={Math.min(data.data.length * 70, 1540)} height={600} data={data.data}>
                    <CartesianGrid vertical={false} strokeDasharray="3 3" />
                    <XAxis dataKey="Name">
                        <Label value="Race" position="bottom" dy={-15} />
                    </XAxis>
                    <YAxis tickCount={10}>
                        <Label
                            value={relative ? "Relatieve Punten" : "Punten"}
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
                    {data.usernames
                        .map((username: string, index) => ({ username, index }))
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
        </div>
    );
};

export default RacePositieScoreChart;
