import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import KlassementenTable from "./KlassementenTable";
import { Rider } from "../../models/Rider";

export type InputData = {
    position: number;
    result: string;
    rider: Rider;
    price: number;
    accounts: string[];
};

const Klassementen = () => {
    document.title = "Klassementen";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<InputData[][]>([]);

    useEffect(() => {
        axios
            .get(`/api/Statistics/klassementen`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            });
    }, [raceId, budgetParticipation]);

    return (
        <div >
            <div style={{ display: 'flex', flexWrap: 'wrap' }}>
                <div key={1} style={{ margin: '5px' }}>
                    <KlassementenTable key="Algemeen" title="Algemeen" riders={data[0]} resultTitle="Tijd" />
                </div>
                <div key={1} style={{ margin: '5px' }}>
                    <KlassementenTable key="Punten" title="Punten" riders={data[1]} resultTitle="Punten" />
                </div>
            </div>
            <div style={{ display: 'flex', flexWrap: 'wrap' }}>
                <div key={1} style={{ margin: '5px' }}>
                    <KlassementenTable key="Berg" title="Berg" riders={data[2]} resultTitle="Punten" />
                </div>
                <div key={1} style={{ margin: '5px' }}>
                    <KlassementenTable key="Jongeren" title="Jongeren" riders={data[3]} resultTitle="Tijd" />
                </div>
            </div>
        </div>
    )
}

export default Klassementen;