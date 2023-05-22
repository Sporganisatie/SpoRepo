import { LineChart, Line, CartesianGrid, XAxis, YAxis, Tooltip, Legend } from 'recharts';
import { useBudgetContext } from '../components/shared/BudgetContextProvider';
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { EtappeUitslag } from './Statistics/EtappeUitslagen/EtappeUitslagenTable';

const colors = ["#00d60e", "#1C43FF", "#FF0000", "#F9F200", "#A900F9", "#FF8000", "#194D33", "#00DEF9", "#F900BB", "#6C3703"];

interface ChartData {
    data: EtappeUitslag[],
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

    const convertData = (data: EtappeUitslag[]): any => { // TODO deze logica naar BE
        const convertedData: any[] = [];
        for (let et = 0; et < data.length; et++) {
            const stageData: { [key: string]: any } = {
                name: data[et].stageNumber
            };

            for (let i = 0; i < data[et].usernamesAndScores.length; i++) {
                const { username, score } = data[et].usernamesAndScores[i];
                stageData[username] = score;
            }
            convertedData.push(stageData)
        }
        return convertedData;
    };

    // const ticks = (): number[] => {
    //     // based on maximun and minimum
    //     return []
    // }

    return (
        <div style={{ backgroundColor: '#222', padding: '20px' }}>
            <LineChart width={800} height={400} data={chartdata.data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis domain={['dataMin -20', 'dataMax +20']} />
                <Tooltip
                    contentStyle={{ backgroundColor: '#333', border: 'none' }}
                    labelStyle={{ color: '#fff' }} />
                <Legend />
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
