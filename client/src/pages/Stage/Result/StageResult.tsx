import { useState } from "react";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import StageClassifications from "./Classifications";
import TeamComparison from "../../../components/shared/Comparison/TeamComparison";
import { useStageResult } from "./StageResultHook";
import { useStage } from "../StageHook";
import Modal from "../../../components/ui/modal/Modal";
import router from "../../../Pages";
import { useRaceContext } from "../../../components/shared/RaceContextProvider";
import StageNav from "../StageNav";
import "../Selection/StageSelection.css";
import "./StageResult.css";

const StageResult = () => {
  const raceId = useRaceContext();
  const { stagenr } = useStage();
  document.title = `Etappe ${stagenr} resultaten`;
  const { data } = useStageResult();
  const [showTeamComparison, setShowTeamComparison] = useState<boolean>(false);

  if (!data) {
    return <div className="stage-selection-page stage-result-page" />;
  }

  return (
    <div className="stage-selection-page stage-result-page">
      <div className="ss-page-header">
        <StageNav />
        <button className="ss-page-cta" onClick={() => setShowTeamComparison(true)}>
          Alle Opstellingen
        </button>
        {data.virtualResult && (
          <div className="sr-virtual-banner">
            Virtuele eindpunten o.b.v. de recentste etappe uitslag.
          </div>
        )}
        <div className="ss-page-header-right">
          {data.finalStandings && (
            <button
              className="ss-page-cta"
              onClick={() => router.navigate(`/${raceId}/racewrap`)}
            >
              Racewrap
            </button>
          )}
        </div>
      </div>

      <div className="ss-body">
        <div className="sr-left-column">
          <div className="ts-panel">
            <TeamResultsTable data={data.teamResult} />
          </div>
          <div className="ts-panel">
            <div className="ts-panel-header">
              <h3 className="ts-panel-title">Poule stand</h3>
            </div>
            <UserScoreTable data={data.userScores} />
          </div>
        </div>
        <StageClassifications
          data={data.classifications}
          finalStandings={data.finalStandings}
        />
      </div>

      <Modal
        open={showTeamComparison}
        modalContents={<TeamComparison />}
        closeFn={() => setShowTeamComparison(false)}
        title="Alle opstellingen"
      />
    </div>
  );
};

export default StageResult;
