import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import { StageSelectionData } from "../../teamselection/Models/StageSelectionData";

const options: Intl.DateTimeFormatOptions = { weekday: 'long', day: 'numeric', month: 'long', hour: 'numeric', minute: 'numeric' };

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} opstelling`;
    const budgetParticipation = useBudgetContext();
    let navigate = useNavigate();
    const [data, setData] = useState<StageSelectionData>({ team: [], deadline: null });

    const loadData = () => {
        axios.get(`/api/StageSelection`, { params: { raceId, stagenr, budgetParticipation } })
            .then(res => {
                setData({ team: res.data.team, deadline: new Date(res.data.deadline) }) // TODO date handling in axios interceptor
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
            {stagenr === "1" && <button onClick={() => navigate("/")}>Teamselectie</button>}
            <div>Deadline: {data.deadline?.toLocaleDateString('nl-NL', options) ?? ""}</div>
            <StageSelectionTeam data={data.team} updateRider={updateRider} loading={false} />
            {/* <TopKlassementen /> */}
        </div>
    )
}

export default StageSelection;