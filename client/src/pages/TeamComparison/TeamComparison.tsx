// import axios from "axios";
import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { SelectOption } from "../../components/Select";
import ArrowSelect from "../../components/ArrowSelect";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import TeamComparisonTable from "./TeamComparisonTable";
import { UserSelection } from "../../models/UserSelection";

const stages: SelectOption<string>[] = Array.from({ length: 21 }, (_, i) => ({
    displayValue: (i + 1).toString(),
    value: (i + 1).toString(),
}));

const TeamComparison = () => {
    let navigate = useNavigate();
    let { raceId, stagenr } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<UserSelection[]>([]);


    useEffect(() => {
        axios.get(`/api/stageresult/comparison`, { params: { raceId, stagenr, budgetParticipation } })
            .then(res => {
                setData(res.data)
            })
            .catch(function (error) {
                throw error
            });
    }, [raceId, stagenr, budgetParticipation, navigate])

    return (
        <div >
            <button style={{ width: 100 }} onClick={() => navigate(`/teamcomparison/${raceId}/1`)}>Etappe opstellingen</button>
            <button style={{ width: 100 }} onClick={() => navigate(`/teamcomparison/${raceId}/`)}>Team selecties</button>
            <ArrowSelect
                value={stagenr}
                allowLooping={false}
                options={stages}
                onChange={(selectedValue) => navigate(`/teamcomparison/${raceId}/${selectedValue}`)} />
            <div style={{ display: 'flex', flexWrap: 'wrap' }}>
                {data.map((userSelection, index) => (
                    <div key={index} style={{ flex: '0 0 24%', marginRight: '2px', marginBottom: '2px' }}>
                        <TeamComparisonTable key={index} username={userSelection.username} riders={userSelection.riders} />
                    </div>
                ))}
            </div>
        </div>
    )
}

export default TeamComparison;