import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import { StageResultData } from "../models/StageResultData";
import StageClassifications from "./Classifications";
import TeamComparison from "../../../components/shared/Comparison/TeamComparison";

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} resultaten`;
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<StageResultData>({ userScores: [], teamResult: [], classifications: { gc: [], points: [], kom: [], youth: [] } });
    const [showTeamComparison, setShowTeamComparison] = useState<boolean>(false)
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
        <div>
            <div style={{ display: 'grid' }}>
                <button
                    style={{ marginRight: 'auto' }}
                    className={showTeamComparison ? 'active' : ''}
                    onClick={() => setShowTeamComparison(!showTeamComparison)}
                >
                    Alle Opstellingen
                </button>
                <div style={{ display: showTeamComparison ? 'block' : 'none' }}>
                    <TeamComparison raceId={raceId ?? ""} stagenr={stagenr} />
                </div>
            </div>
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
        </div>
    );
}

export default StageSelection;