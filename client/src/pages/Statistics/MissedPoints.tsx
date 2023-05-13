import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import MissedPointsTable from "./MissedPointsTable";

export type MissedPointsData = {
    etappe: number;
    behaald: number;
    optimaal: number;
    gemist: number;
};

type MissedPointsTableData = {
    username: string;
    data: MissedPointsData[];
};

const MissedPoints = () => {
    document.title = "Gemiste punten";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<MissedPointsTableData[]>([]);

    useEffect(() => {
        axios
            .get(`/api/Statistics/missedPoints`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
                document.title = "Zeg tegen Rens Error bij gemiste punten";
            });
    }, [raceId, budgetParticipation]);

    return (
        <div >
            {<div style={{ display: 'flex', flexWrap: 'wrap' }}>
                {data.map((missedPoints, index) => (
                    <div key={index} style={{ margin: '5px' }}>
                        <MissedPointsTable key={index} title={missedPoints.username} riders={missedPoints.data} />
                    </div>
                ))}
            </div>}
        </div>
    )
}

export default MissedPoints;