import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useNavigate } from "react-router-dom";
import ArrowSelect from "../../../components/ArrowSelect";
import { SelectOption } from "../../../components/Select";
// import { StageSelectionData } from "../../teamselection/Models/StageSelectionData";
// import TeamResultsTable, { Teamresults } from "./TeamResultsTable";
import UserScoreTable, { UserScore } from "./UserScoreTable";

const stages: SelectOption<string>[] = Array.from({ length: 21 }, (_, i) => ({
    displayValue: (i + 1).toString(),
    value: (i + 1).toString(),
}));

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} resultaten`;
    const budgetParticipation = useBudgetContext();
    let navigate = useNavigate();
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
            {stagenr === "1" && <button onClick={() => navigate("/")}>Teamselectie</button>}
            <ArrowSelect
                value={stagenr}
                allowLooping={false}
                options={stages}
                onChange={(selectedValue) => { navigate(`/stage/${raceId}/${selectedValue}`) }} />
            {/* <TeamResultsTable data={[]} /> */}
            <UserScoreTable data={userScoreData} />
        </div>
    )
}

export default StageSelection;