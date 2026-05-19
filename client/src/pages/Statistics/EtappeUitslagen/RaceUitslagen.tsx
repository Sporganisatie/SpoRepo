import { useEffect, useState } from "react";
import axios from "../../../api/client";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import UitslagenTable from "./UitslagenTable";
import ScoreverdelingTable from "./ScoreverdelingTable";
import RankCountTable from "./RankCountTable";
import { useParams } from "react-router-dom";

const RaceUitslagen = () => {
  document.title = "Race uitslagen";
  const budgetParticipation = useBudgetContext();
  const { raceName } = useParams();
  const [data, setData] = useState<{ uitslagen: any[]; scoreVerdeling: any[]; userRanks: any[] }>({
    uitslagen: [],
    scoreVerdeling: [],
    userRanks: [],
  });

  useEffect(() => {
    axios
      .get(`/api/Statistics/raceUitslagen`, { params: { budgetParticipation, raceName } })
      .then((res) => {
        setData(res.data);
      })
      .catch((error) => {});
  }, [budgetParticipation, raceName]);

  return (
    <div className="h-stack">
      <div style={{ flex: 1 }}>
        <UitslagenTable data={data.uitslagen} allRaces={true} />
      </div>
      <div className="v-stack">
        <ScoreverdelingTable data={data.scoreVerdeling} allRaces={true} />
        <RankCountTable data={data.userRanks} />
      </div>
    </div>
  );
};

export default RaceUitslagen;
