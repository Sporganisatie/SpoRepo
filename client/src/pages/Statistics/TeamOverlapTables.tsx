import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import UniekheidTable from "./UniekheidTabel";
import axios from "axios";

export type UniekheidData = {
    user: string;
    uniekheid: number;
};

const TeamOverlapTables = ({ includeDnfRiders }: { includeDnfRiders: boolean }) => {
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<UniekheidData[]>([]);

    useEffect(() => {
        console.log("called uniekheid endpoint")
        axios
            .get(`/api/Statistics/Uniekheid`, { params: { raceId, budgetParticipation, includeDnf: includeDnfRiders } })
            .then(res => {
                setData(res.data);
            })
            .catch((error) => {
                throw error;
            });
    }, []);

    return (
        <div>
            <div style={{ fontSize: '32px', textAlign: 'center', margin: '20px 0' }}>
                {includeDnfRiders ? 'Aan de start' : 'Nu'}
            </div>
            <div style={{ display: 'flex', flexWrap: 'wrap' }}>
                <div key={1} style={{ margin: '5px' }}>
                    <UniekheidTable userUniekheden={data} />
                </div>
            </div>
        </div>
    )
}

export default TeamOverlapTables;