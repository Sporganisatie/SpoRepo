import { useEffect, useState } from "react";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import Overlaptable from "./OverlapTable";
import { useParams } from "react-router-dom";
import axios from "axios";

export type OverlapData = {
    overlap: OverlapRow[];
    overlapBudget: OverlapRow[];
};

export type OverlapRow = {
    user: string;
    overlaps: { [id: string]: number }
};

const Overlap = () => {
    document.title = "Team Overlap";

    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<OverlapData>({ overlap: [], overlapBudget: [] });

    useEffect(() => {
        axios
            .get(`/api/Statistics/Overlap`, {
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
            <Overlaptable title="Overlap (Renners)" overlaps={data.overlap} />
            <Overlaptable title="Overlap (% budget)" overlaps={data.overlapBudget} />
        </div>
    )
}

export default Overlap;