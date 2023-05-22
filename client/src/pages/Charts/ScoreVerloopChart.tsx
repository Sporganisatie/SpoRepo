import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from 'recharts';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { colors, convertData } from './ChartsHelper';

interface ChartData {
    data: any[],
    usernames: string[]
}

const ScoreVerloopChart = () => {
    document.title = "Score Verloop";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });

    useEffect(() => {
        axios.get(`/api/Charts/scoreVerloop`, { params: { raceId, budgetParticipation } })
            .then(res => {
                const usernames = res.data[0].usernamesAndScores.map((x: { username: string; }) => x.username);
                setChartData({ data: convertData(res.data), usernames });
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation]);

    return (
        <div style={{ backgroundColor: '#222', padding: '20px' }}>
            <LineChart width={chartdata.data.length * 70} height={600} data={chartdata.data}>
                <CartesianGrid vertical={false} strokeDasharray="3 3" />
                <XAxis dataKey="name" >
                    <Label
                        value="Etappe"
                        position="bottom"
                        dy={-15}
                    />
                </XAxis>
                <YAxis tickCount={10}>
                    <Label
                        value="Relatieve Punten"
                        angle={-90}
                        position="left"
                        offset={-10}
                    />
                </YAxis>
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
                        isAnimationActive={false}
                    />
                ))}
            </LineChart>
        </div>
    );
};

export default ScoreVerloopChart;