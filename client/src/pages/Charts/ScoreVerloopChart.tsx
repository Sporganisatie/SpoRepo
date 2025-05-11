import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from 'recharts';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { colors } from './ChartsHelper';
import SmallSwitch from '../../components/shared/SmallSwitch';

interface ChartData {
    data: any[],
    usernames: string[]
}

const ScoreVerloopChart = () => {
    document.title = "Score Verloop";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<ChartData>({ data: [], usernames: [] });
    const [filteredData, setFilteredData] = useState<any[]>([]);
    const [positieVerloop, setPositieVerloop] = useState<boolean>(false);
    const [perfectPoints, setPerfectPoints] = useState<boolean>(false);
    const [filters, setFilters] = useState<{ toggles: { username: string, showUser: boolean }[], center: string }>({ toggles: [], center: "Gemiddelde" });

    useEffect(() => {
        var url = positieVerloop ? "positieVerloop" : perfectPoints ? "perfectScoreVerloop" : "scoreVerloop";
        axios.get(`/api/Charts/${url}`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData({ data: res.data.data, usernames: res.data.users });
                const toggles = res.data.users.map((username: string) => ({ username, showUser: true }));
                setFilters({ toggles, center: "Gemiddelde" });
                setFilteredData(res.data.data);
            })
    }, [raceId, budgetParticipation, positieVerloop, perfectPoints]);

    const setToggles = (toggles: any): void => {
        setFilters((prevfilters) => {
            return {
                ...prevfilters,
                toggles: toggles
            };
        })
    }

    const setCenter = (value: string): void => {
        setFilters((prevfilters) => {
            return {
                ...prevfilters,
                center: value
            };
        })
    }

    const toggleUser = (index: number): void => {
        const newToggles = filters.toggles;
        newToggles[index].showUser = !newToggles[index].showUser;
        if (filters.center === newToggles[index].username) {
            setFilters({ toggles: newToggles, center: "Gemiddelde" })
        }
        else {
            setToggles(newToggles)
        }
    }

    const toggleAll = (): void => {
        const newValue = filters.toggles.some(toggle => !toggle.showUser);
        const newToggles = filters.toggles.map(toggle => ({ ...toggle, showUser: newValue }));
        if (!newValue) {
            setFilters({ toggles: newToggles, center: "Gemiddelde" })
        }
        else {
            setToggles(newToggles)
        }
    }

    const updateFiltered = useCallback(
        (): void => {
            const filteredData: any[] = [];
            if (filters.toggles.every(toggle => !toggle.showUser)) {
                setFilteredData(filteredData);
                return;
            }
            data.data.forEach((scores) => {
                let total = 0;
                let min = Number.MAX_VALUE;
                let max = Number.MIN_VALUE;
                let userValue = 0;
                Object.entries(scores).forEach(([key, value], index) => {
                    if (index > 0 && filters.toggles[index - 1].showUser) {
                        const numericValue = Number.parseInt(`${value}`);
                        total += numericValue;
                        min = Math.min(min, numericValue);
                        max = Math.max(max, numericValue);
                        if (key === filters.center) {
                            userValue = numericValue;
                        }
                    }
                });

                let offset = Math.floor(total / filters.toggles.filter((x) => x.showUser).length);
                switch (filters.center) {
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
            setFilteredData(filteredData);
        },
        [filters, data.data]
    );

    useEffect(() => {
        updateFiltered();
    }, [updateFiltered]);

    const togglePositieVerloop = () => {
        setPositieVerloop(!positieVerloop)
        if (!positieVerloop) {
            setPerfectPoints(false)
        }
    }

    const togglePerfectPoints = () => {
        setPerfectPoints(!perfectPoints)
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
                {filters.toggles.map((user, index) => (
                    <SmallSwitch key={index} text={user.username} selected={user.showUser} index={index} toggleUser={() => toggleUser(index)} />
                ))}
                <div style={{ display: 'inline-block', marginLeft: "5px", marginBottom: "5px" }}>
                    <button onClick={toggleAll}>Toggle alle</button>
                </div>
                <div style={{ display: 'inline-block', marginLeft: "5px", marginBottom: "5px" }}>
                    <select value={filters.center} onChange={(e) => setCenter(e.target.value)}>
                        <option value="Gemiddelde">Gemiddelde</option>
                        <option value="Min">Min</option>
                        <option value="Max">Max</option>
                        {filters.toggles.filter((user) => user.showUser).map((user, index) => (
                            <option key={index} value={user.username}>{user.username}</option>
                        ))}
                    </select>
                </div>
            </div>}
            <div>
                <LineChart
                    width={Math.min(data.data.length * 70, 1540)}
                    height={600}
                    data={filteredData}>
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
                        .filter((_, index) => filters.toggles.at(index)?.showUser).map(user => (
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
