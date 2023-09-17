import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label } from 'recharts';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { useEffect, useState } from "react";
import axios from "axios";
import { colors } from './ChartsHelper';

interface ChartData {
    data: any[],
    usernames: string[]
}

const RaceScoreVerloopChart = () => {
    document.title = "Score Verloop";
    const budgetParticipation = useBudgetContext();
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });

    useEffect(() => {
        axios.get(`/api/Charts/raceScoreVerloop`, { params: { budgetParticipation } })
            .then(res => {
                setChartData({ data: res.data.data, usernames: res.data.users });
            })
            .catch(error => {
            });
    }, [budgetParticipation]);

    return (
        <div style={{ backgroundColor: '#222', padding: '20px' }}>
            <LineChart width={1560} height={600} data={chartdata.data}>
                <CartesianGrid vertical={false} strokeDasharray="3 3" />
                <XAxis dataKey="Name" >
                    <Label
                        value="Race"
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
                    />
                ))}
            </LineChart>
        </div>
    );
};

export default RaceScoreVerloopChart;
