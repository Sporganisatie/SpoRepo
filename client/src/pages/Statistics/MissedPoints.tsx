import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";

const MissedPoints = () => {
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<[]>([]);

    useEffect(() => {
        axios
            .get(`/api/Statistics/missedPoints`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
                throw error;
            });
    }, [raceId, budgetParticipation]);

    return (
        // put data into a DataTable
        <div>Missed points voor {raceId}, data: {data}</div>
    )
}

export default MissedPoints;