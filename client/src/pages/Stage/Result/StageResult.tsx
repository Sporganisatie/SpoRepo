import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import { StageResultData } from "../models/StageResultData";

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} resultaten`;
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<StageResultData>({ userScores: [], teamResult: [] });

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
            <TeamResultsTable data={data.teamResult} />
            <UserScoreTable data={data.userScores} />
        </div>
    )
}

export default StageSelection;