import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import EtappeUitslagenTable from "./EtappeUitslagenTable";
import ScoreverdelingTable from "./ScoreverdelingTable";

const EtappeUitslagen = () => {
    document.title = "Etappe uitslagen";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<{ uitslagen: any[], scoreVerdeling: any[] }>({ uitslagen: [], scoreVerdeling: [] });

    useEffect(() => {
        axios
            .get(`/api/Statistics/etappeUitslagen`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation]);

    return (
        <div >
            <EtappeUitslagenTable data={data.uitslagen} />
            <ScoreverdelingTable data={data.scoreVerdeling} />
        </div>
    )
}

export default EtappeUitslagen;