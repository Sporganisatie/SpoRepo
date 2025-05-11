import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from 'recharts';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { colors } from './ChartsHelper';
import SmallSwitch from '../../components/shared/SmallSwitch';
import { number } from 'zod';

interface ChartData {
    data: any[],
    usernames: string[]
}

const ScoreVerloopChart = () => {
    document.title = "Score Verloop";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<ChartData>({ data: [], usernames: [] });
    const [filtered, setFiltered] = useState<any[]>([]);
    const [positieVerloop, setPositieVerloop] = useState<boolean>(false);
    const [perfectPoints, setPerfectPoints] = useState<boolean>(false);
    const [toggles, setToggles] = useState<{ username: string, showUser: boolean }[]>([]);
    const [centerValue, setCenterValue] = useState<string>("Gemiddelde");

    useEffect(() => {
        var url = positieVerloop ? "positieVerloop" : perfectPoints ? "perfectScoreVerloop" : "scoreVerloop";
        axios.get(`/api/Charts/${url}`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData({ data: res.data.data, usernames: res.data.users });
                const toggles = res.data.users.map((username: string) => ({ username, showUser: true }));
                setToggles(toggles);
                setFiltered(res.data.data);
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation, positieVerloop, perfectPoints]);


    const toggleUser = (index: number): void => {
        setToggles((prevToggles) => {
            const newToggles = [...prevToggles];
            newToggles[index].showUser = !newToggles[index].showUser;
            if (centerValue === newToggles[index].username) {
                setCenterValue("Gemiddelde");
            }
            return newToggles;
        });
    }

    const toggleAll = (): void => {
        setToggles((prevToggles) => {
            const newValue = prevToggles.some(toggle => !toggle.showUser);
            const newToggles = prevToggles.map(toggle => ({ ...toggle, showUser: newValue }));
            return newToggles;
        });
    }

    const updateFiltered = useCallback(
        (newToggles: { username: string; showUser: boolean }[]): void => {
            const filteredData: any[] = [];
            data.data.forEach((scores) => {
                let total = 0;
                let min = Number.MAX_VALUE;
                let max = Number.MIN_VALUE;
                let userValue = 0;
                Object.entries(scores).forEach(([key, value], index) => {
                    if (index > 0 && newToggles[index - 1].showUser) {
                        const numericValue = Number.parseInt(`${value}`);
                        total += numericValue;
                        min = Math.min(min, numericValue);
                        max = Math.max(max, numericValue);
                        if (key === centerValue) {
                            userValue = numericValue;
                        }
                    }
                });

                let offset = Math.floor(total / newToggles.filter((x) => x.showUser).length);
                switch (centerValue) {
                    case 'Gemiddelde':
                        break;
                    case 'Min':
                        offset = min;
                        break;
                    case 'Max':
                        offset = max;
                        break;
                    default:
                        offset = userValue;
                        break;
                }

                const newValue = new Map();
                Object.entries(scores).forEach(([key, value]) => {
                    newValue.set(key, Number(value) - offset);
                });
                newValue.set("Name", scores.Name.toString());
                filteredData.push(Object.fromEntries(newValue));
            });
            console.log("filteredData", filteredData);
            setFiltered(filteredData);
        },
        [data.data, centerValue, setFiltered]
    );

    useEffect(() => {
        updateFiltered(toggles);
    }, [centerValue, toggles, updateFiltered]);

    const togglePositieVerloop = () => {
        setPositieVerloop(!positieVerloop)
        if (!positieVerloop) {
            setPerfectPoints(false)
        }
    }

    const togglePerfectPoints = () => {
        setPerfectPoints(!perfectPoints)
    }

    function setCenter(value: string): void {
        setCenterValue(value);
    }

    return (
        <div style={{ backgroundColor: '#222', padding: '20px' }}>
            <div>
                <div style={{ display: 'inline-block', cursor: 'pointer', color: 'white' }} onClick={togglePositieVerloop}>
                    Positie Verloop
                    <input type="checkbox" checked={positieVerloop} onChange={() => { }} />
                </div>
                {!positieVerloop && (
                    <div style={{ display: 'inline-block', cursor: 'pointer', color: 'white' }} onClick={togglePerfectPoints}>
                        Perfecte Score
                        <input type="checkbox" checked={perfectPoints} onChange={() => { }} />
                    </div>
                )}
            </div>
            {!positieVerloop && <div>
                {toggles.map((user, index) => (
                    <SmallSwitch key={index} text={user.username} selected={user.showUser} index={index} toggleUser={() => toggleUser(index)} />
                ))}
                <div style={{ display: 'inline-block', marginLeft: "5px", marginBottom: "5px" }}>
                    <button onClick={toggleAll}>Toggle alle</button>
                </div>
                <div style={{ display: 'inline-block', marginLeft: "5px", marginBottom: "5px" }}>
                    <select value={centerValue} onChange={(e) => setCenter(e.target.value)}>
                        <option value="Gemiddelde">Gemiddelde</option>
                        <option value="Min">Min</option>
                        <option value="Max">Max</option>
                        {toggles.filter((user) => user.showUser).map((user, index) => (
                            <option key={index} value={user.username}>{user.username}</option>
                        ))}
                    </select>
                </div>
            </div>}
            <div>
                <LineChart
                    width={Math.min(data.data.length * 70, 1540)}
                    height={600}
                    data={filtered}>
                    <CartesianGrid vertical={false} strokeDasharray="3 3" />
                    <XAxis dataKey="Name" >
                        <Label
                            value="Etappe"
                            position="bottom"
                            dy={-15}
                        />
                    </XAxis>
                    {positieVerloop ? (
                        <YAxis
                            tickCount={10}
                            tickFormatter={(value) => (-value).toString()}
                        >
                            <Label value="Positie" angle={-90} position="left" offset={-10} />
                        </YAxis>
                    ) : (
                        <YAxis tickCount={10}>
                            <Label
                                value={positieVerloop ? "Positie" : "Relatieve Punten"}
                                angle={-90}
                                position="left"
                                offset={-10}
                            />
                        </YAxis>
                    )}
                    <Tooltip
                        contentStyle={{ backgroundColor: '#333', border: 'none' }}
                        labelStyle={{ color: '#fff' }} />
                    <Legend
                        verticalAlign="top"
                        wrapperStyle={{ marginTop: -10 }}
                    />
                    {data.usernames.map((username: string, index) => ({ username, index }))
                        .filter((_, index) => toggles.at(index)?.showUser).map(user => (
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

export default ScoreVerloopChart;
