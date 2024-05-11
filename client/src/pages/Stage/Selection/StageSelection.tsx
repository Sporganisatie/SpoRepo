import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import { StageSelectionData } from "../models/StageSelectionData";
import ClassificationOverview from "./ClassificationOverview";
import SelectionsComplete from "./SelectionComplete";
import RulesPopup from "../../RulesPopup";

const options: Intl.DateTimeFormatOptions = { weekday: 'long', day: 'numeric', month: 'long', hour: 'numeric', minute: 'numeric' };

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} opstelling`;
    const budgetParticipation = useBudgetContext();
    let navigate = useNavigate();
    const [data, setData] = useState<StageSelectionData>({ team: [], deadline: null, classifications: { gc: [], points: [], kom: [], youth: [] }, compleet: 0, budgetCompleet: null });

    const loadData = () => {
        axios.get(`/api/StageSelection`, { params: { raceId, stagenr, budgetParticipation } })
            .then(res => {
                var deadline = new Date(res.data.deadline); // TODO date handling in axios interceptor
                deadline.setHours(deadline.getHours() + 2)
                setData({ team: res.data.team, deadline, classifications: res.data.classifications, compleet: res.data.compleet, budgetCompleet: res.data.budgetCompleet })
            })
            .catch(function (error) {
                throw error
            });
    }

    /* eslint-disable */
    useEffect(() => { loadData() }, [raceId, stagenr, budgetParticipation])
    /* eslint-enable */

    const updateRider = (riderParticipationId: number, isAdding: boolean, type: string) => {
        // setPending(true);
        axios.request({
            method: isAdding ? "post" : "delete",
            url: "/api/StageSelection/" + type,
            params: {
                riderParticipationId,
                raceId,
                budgetParticipation,
                stagenr
            }
        })
            .then(function (response) {
                loadData();
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    return (
        <div>
            <div style={{ margin: "10px 0" }}>Deadline: {data.deadline?.toLocaleDateString('nl-NL', options) ?? ""}</div>
            {stagenr === "1" && <button onClick={() => navigate("/")}>Teamselectie</button>}
            <SelectionsComplete compleet={data.compleet} budgetCompleet={data.budgetCompleet} />
            <div style={{ display: "flex" }}>
                <div style={{ margin: "0 5px" }}>
                    <StageSelectionTeam data={data.team} updateRider={updateRider} loading={false} />
                </div>
                <div style={{ margin: "0 5px" }}>
                    <ClassificationOverview data={data.classifications} />
                </div>
            </div>
            <RulesPopup page={"stageSelection"} />
        </div>
    );


}

export default StageSelection;