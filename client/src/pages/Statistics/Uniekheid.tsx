import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import UniekheidTable, { UniekheidRow } from "./UniekheidTable";

export type UniekheidData = {
    start: UniekheidRow[];
    huidig: UniekheidRow[];
};

const Uniekheid = () => {
    document.title = "Uniekheid";

    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<UniekheidData>({ start: [], huidig: [] });

    useEffect(() => {
        axios
            .get(`/api/Statistics/Uniekheid`, {
                params: { raceId, budgetParticipation },
            })
            .then((res) => {
                setData(res.data);
            })
            .catch((error) => {
                throw error;
            });
    }, [raceId, budgetParticipation]);

    return (
        <div style={{ gap: "10px", display: "flex" }}>
            <UniekheidTable title="Aan de start" data={data.start} />
            <UniekheidTable title="Nu" data={data.huidig} />
        </div>
    )
}

export default Uniekheid;