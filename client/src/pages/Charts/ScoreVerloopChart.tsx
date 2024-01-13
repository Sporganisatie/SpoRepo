import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from 'recharts';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { colors } from './ChartsHelper';

interface ChartData {
    data: any[],
    usernames: string[]
}

const ScoreVerloopChart = () => {
    document.title = "Score Verloop";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });
    const [positieVerloop, setPositieVerloop] = useState<boolean>(false);
    const [perfectPoints, setPerfectPoints] = useState<boolean>(false);

    useEffect(() => {
        var url = positieVerloop ? "positieVerloop" : perfectPoints ? "perfectScoreVerloop" : "scoreVerloop";
        axios.get(`/api/Charts/${url}`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setChartData({ data: res.data.data, usernames: res.data.users });
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation, positieVerloop, perfectPoints]);

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
            <div>
                <LineChart width={Math.min(chartdata.data.length * 70, 1540)} height={600} data={chartdata.data}>
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
        </div>
    );
};

export default ScoreVerloopChart;
