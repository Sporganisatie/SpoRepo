import { CartesianGrid, XAxis, YAxis, Tooltip, Legend, Label, Bar, BarChart } from 'recharts';
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import { colors, convertData } from './ChartsHelper';


interface ChartData {
    data: [],
    usernames: string[]
}

const ScoreVerdelingChart = () => {
    document.title = "Scores per Etappe";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [chartdata, setChartData] = useState<ChartData>({ data: [], usernames: [] });

    useEffect(() => {
        axios.get(`/api/Charts/scoreSpread`, { params: { raceId, budgetParticipation } })
            .then(res => {
                const usernames = res.data[0].usernamesAndScores.map((x: { username: string; }) => x.username);
                setChartData({ data: convertData(res.data), usernames });
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation]);

    const CalcVerticalLines = (): number[] => {
        // bereken de posities tussen de bar groepen aan de hand van chart size en aantal etappes en users
        return []
    }

    return (
        <div style={{ backgroundColor: '#222', padding: '20px' }}>
            <BarChart width={chartdata.data.length * 70} height={600} data={chartdata.data}>
                <CartesianGrid strokeDasharray="3 3" verticalPoints={CalcVerticalLines()} vertical={false} />
                <XAxis dataKey="name">
                    <Label
                        value="Etappe"
                        position="bottom"
                        dy={-15}
                    />
                </XAxis>
                <YAxis tickCount={10}>
                    <Label
                        value="Score"
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
                    <Bar
                        key={index}
                        dataKey={username}
                        fill={colors[index]}
                        isAnimationActive={false}
                    />
                ))}
            </BarChart>
        </div>
    );
};

export default ScoreVerdelingChart;
