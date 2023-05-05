import axios from "axios";
import { useEffect, useState } from "react";
import { StageSelectableRider } from "../models/StageSelectableRider";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import ArrowSelect from "../../../components/ArrowSelect";
import { SelectOption } from "../../../components/Select";

const stages: SelectOption<string>[] = Array.from({ length: 21 }, (_, i) => ({
    displayValue: (i + 1).toString(),
    value: (i + 1).toString(),
}));

const StageSelection = (props: { raceId: string, stagenr: string }) => {
    const { raceId, stagenr } = props;
    document.title = `Etappe ${stagenr} opstelling`;
    const budgetParticipation = useBudgetContext();
    let navigate = useNavigate();
    const [data, setData] = useState<StageSelectableRider[]>([]);

    const loadData = () => {
        axios.get(`/api/StageSelection`, { params: { raceId, stagenr, budgetParticipation } })
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
            <ArrowSelect
                value={stagenr}
                allowLooping={false}
                options={stages}
                onChange={(selectedValue) => { navigate(`/stage/${raceId}/${selectedValue}`) }} />
            <StageSelectionTeam data={data} updateRider={updateRider} loading={false} />
            {/* <TopKlassementen /> */}
        </div>
    )
}

export default StageSelection;