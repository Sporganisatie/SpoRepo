import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "../../../api/client";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import UitslagenTable from "./UitslagenTable";
import ScoreverdelingTable from "./ScoreverdelingTable";
import RankCountTable from "./RankCountTable";

const EtappeUitslagen = () => {
  document.title = "Etappe uitslagen";
  const { raceId } = useParams();
  const budgetParticipation = useBudgetContext();
  const [data, setData] = useState<{ uitslagen: any[]; scoreVerdeling: any[]; userRanks: any[] }>({
    uitslagen: [],
    scoreVerdeling: [],
    userRanks: [],
  });

  useEffect(() => {
    axios
      .get(`/api/Statistics/etappeUitslagen`, { params: { raceId, budgetParticipation } })
      .then((res) => {
        setData(res.data);
      })
      .catch((error) => {});
  }, [raceId, budgetParticipation]);

  return (
    <div style={{ display: "flex" }}>
      <div style={{ flex: "1" }}>
        <UitslagenTable data={data.uitslagen} allRaces={false} />
      </div>
      <div style={{ marginLeft: "10px" }}>
        <div style={{ marginBottom: "10px" }}>
          <ScoreverdelingTable data={data.scoreVerdeling} allRaces={false} />
        </div>
        <RankCountTable data={data.userRanks} />
      </div>
    </div>
  );
};

export default EtappeUitslagen;
