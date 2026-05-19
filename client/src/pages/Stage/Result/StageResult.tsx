import { useState } from "react";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import StageClassifications from "./Classifications";
import TeamComparison from "../../../components/shared/Comparison/TeamComparison";
import { useStageResult } from "./StageResultHook";
import { useStage } from "../StageHook";
import Modal from "../../../components/ui/modal/Modal";
import StageProfiles from "../StageProfiles";
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
  const [showProfiles, setShowProfiles] = useState<boolean>(false);

  if (!data) {
    return <div className="stage-selection-page stage-result-page" />;
  }

  return (
    <div className="stage-selection-page stage-result-page">
      <div className="stage-select-page-header">
        <StageNav />
        <button className="stage-select-page-cta" onClick={() => setShowProfiles(true)}>
          Profielen
        </button>
        <button className="stage-select-page-cta" onClick={() => setShowTeamComparison(true)}>
          Opstellingen
        </button>
        {data.virtualResult && (
          <div className="stage-result-virtual-banner">
            Virtuele eindpunten o.b.v. de recentste etappe uitslag.
          </div>
        )}
        <div className="stage-select-page-header-right">
          {data.finalStandings && (
            <button
              className="stage-select-page-cta"
              onClick={() => router.navigate(`/${raceId}/racewrap`)}
            >
              Racewrap
            </button>
          )}
        </div>
      </div>

      <div className="stage-select-body">
        <div className="stage-result-left-column">
          <div className="panel stage-result-team-results-panel">
            <TeamResultsTable data={data.teamResult} />
          </div>
          <div className="panel">
            <div className="panel-header">
              <h3 className="panel-title">Poule stand</h3>
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
        title="Opstellingen"
      />
      <Modal
        open={showProfiles}
        modalContents={<StageProfiles raceId={raceId.toString()} stageNr={stagenr} />}
        closeFn={() => setShowProfiles(false)}
        title="Profielen"
      />
    </div>
  );
};

export default StageResult;
