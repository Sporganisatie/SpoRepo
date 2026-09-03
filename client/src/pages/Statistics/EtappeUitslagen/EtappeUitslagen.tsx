import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "../../../api/client";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import UitslagenTable from "./UitslagenTable";
import ScoreverdelingTable from "./ScoreverdelingTable";
import RankCountTable from "./RankCountTable";
import StageScoreSpreadChart from "./StageScoreSpreadChart";
import "./StageScoreSpreadChart.css";

interface UserStageScores {
  username: string;
  stageScores: number[];
}

interface UsernameScore {
  username: string;
  score: number;
}

interface EtappeUitslag {
  usernamesAndScores: UsernameScore[];
  stageNumber: string;
}

interface ScoreVerdeling {
  username: string;
  bin0: number;
  bin1: number;
  bin2: number;
  bin3: number;
  bin4: number;
}

interface UserRank {
  username: string;
  ranks: number[];
}

const EtappeUitslagen = () => {
  const { raceId } = useParams();
  const budgetParticipation = useBudgetContext();
  const [data, setData] = useState<{
    uitslagen: EtappeUitslag[];
    scoreVerdeling: ScoreVerdeling[];
    userRanks: UserRank[];
    stageScoreSpread: UserStageScores[];
  }>({
    uitslagen: [],
    scoreVerdeling: [],
    userRanks: [],
    stageScoreSpread: [],
  });

  useEffect(() => {
    document.title = "Etappe uitslagen";
  }, []);

  useEffect(() => {
    axios
      .get(`/api/Statistics/etappeUitslagen`, { params: { raceId, budgetParticipation } })
      .then((res) => {
        setData(res.data);
      })
      .catch(() => {});
  }, [raceId, budgetParticipation]);

  return (
    <div className="h-stack">
      <div style={{ flex: 1 }}>
        <UitslagenTable data={data.uitslagen} allRaces={false} />
      </div>
      <div className="v-stack">
        <ScoreverdelingTable data={data.scoreVerdeling} allRaces={false} />
        <RankCountTable data={data.userRanks} />
        <StageScoreSpreadChart data={data.stageScoreSpread} />
      </div>
    </div>
  );
};

export default EtappeUitslagen;
