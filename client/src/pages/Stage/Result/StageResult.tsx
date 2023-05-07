import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
// import { StageSelectionData } from "../../teamselection/Models/StageSelectionData";
// import TeamResultsTable, { Teamresults } from "./TeamResultsTable";
import UserScoreTable, { UserScore } from "./UserScoreTable";

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} resultaten`;
    const budgetParticipation = useBudgetContext();
    // const [teamResultsData, setTeamResultsData] = useState<Teamresults[]>([]);
    const [userScoreData, setUserScoreData] = useState<UserScore[]>([]);

    const loadData = () => {
        axios.get(`/api/stageresults`, { params: { raceId, stagenr, budgetParticipation } })
            .then(res => {
                setUserScoreData(res.data)
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
            {/* <TeamResultsTable data={[]} /> */}
            <UserScoreTable data={userScoreData} />
        </div>
    )
}

export default StageSelection;