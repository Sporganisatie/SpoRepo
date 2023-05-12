import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import { StageResultData } from "../models/StageResultData";
import StageClassifications from "./Classifications";

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} resultaten`;
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<StageResultData>({ userScores: [], teamResult: [], classifications: { gc: [], points: [], kom: [], youth: [] } });

    const loadData = () => {
        axios.get(`/api/stageresult`, { params: { raceId, stagenr, budgetParticipation } })
            .then(res => {
                setData(res.data)
            })
            .catch(function (error) {
                throw error
            });
    }

    /* eslint-disable */
    useEffect(() => { loadData() }, [raceId, stagenr, budgetParticipation])
    /* eslint-enable */

    return (
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px', margin: '10px' }}>
            <div>
                <div style={{ marginBottom: '10px' }}>
                    <TeamResultsTable data={data.teamResult} />
                </div>
                <UserScoreTable data={data.userScores} />
            </div>
            <div>
                <StageClassifications data={data.classifications} />
            </div>
        </div>
    );
}

export default StageSelection;