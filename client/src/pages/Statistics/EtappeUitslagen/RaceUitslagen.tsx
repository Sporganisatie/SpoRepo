import { useEffect, useState } from "react";
import axios from "axios";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import UitslagenTable from "./UitslagenTable";
import ScoreverdelingTable from "./ScoreverdelingTable";
import RankCountTable from "./RankCountTable";

const RaceUitslagen = () => {
    document.title = "Race uitslagen";
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<{ uitslagen: any[], scoreVerdeling: any[], userRanks: any[] }>({ uitslagen: [], scoreVerdeling: [], userRanks: [] });

    useEffect(() => {
        axios
            .get(`/api/Statistics/raceUitslagen`, { params: { budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
            });
    }, [budgetParticipation]);

    return (
        <div style={{ display: 'flex' }}>
            <div style={{ flex: '1' }}>
                <UitslagenTable data={data.uitslagen} allRaces={true} />
            </div>
            <div style={{ marginLeft: '10px' }}>
                <div style={{ marginBottom: '10px' }}>
                    <ScoreverdelingTable data={data.scoreVerdeling} allRaces={true} />
                </div>
                <RankCountTable data={data.userRanks} />
            </div>
        </div>
    );
}

export default RaceUitslagen;