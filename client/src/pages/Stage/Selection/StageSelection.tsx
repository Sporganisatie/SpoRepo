import { useState } from "react";
import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import ClassificationOverview from "./ClassificationOverview";
import { useStageSelection } from "./StageSelectionHook";
import { useStage } from "../StageHook";
import StageNav from "../StageNav";
import CountdownClock24H, { isWithin24h } from "../../teamselection/CountdownClock";
import TeamComparison from "../../../components/shared/Comparison/TeamComparison";
import Modal from "../../../components/ui/modal/Modal";
import StageProfiles from "../StageProfiles";
import "./StageSelection.css";

const dateOptions: Intl.DateTimeFormatOptions = {
  weekday: "long",
  day: "numeric",
  month: "long",
  hour: "numeric",
  minute: "numeric",
};

const StageSelection = () => {
  const { raceId, stagenr } = useStage();
  const [showTeamComparison, setShowTeamComparison] = useState<boolean>(false);
  const [showProfiles, setShowProfiles] = useState<boolean>(false);
  document.title = `Etappe ${stagenr} opstelling`;

  const { data, isLoading, addRider, removeRider, addKopman, removeKopman } = useStageSelection();
  const navigate = useNavigate();

  if (!data) {
    return <div className="stage-selection-page" />;
  }

  const within24h = data.deadline != null && isWithin24h(data.deadline);

  return (
    <div className="stage-selection-page">
      <div className="stage-select-page-header">
        <StageNav />
        <button className="stage-select-page-cta" onClick={() => setShowProfiles(true)}>
          Profielen
        </button>
        {!within24h && (
          <div className="stage-select-deadline">
            {data.deadline?.toLocaleDateString("nl-NL", dateOptions) ?? ""}
          </div>
        )}
        <div className="stage-select-page-header-right">
          {within24h && data.deadline && (
            <CountdownClock24H targetDate={data.deadline} className="compact" />
          )}
          {stagenr === "1" && (
            <button className="stage-select-page-cta" onClick={() => navigate("/")}>
              Teamselectie
            </button>
          )}
        </div>
      </div>

      <div className="stage-select-body">
        <StageSelectionTeam
          team={data.team}
          isFetching={isLoading}
          compleet={data.compleet}
          budgetCompleet={data.budgetCompleet}
          addRider={addRider}
          removeRider={removeRider}
          addKopman={addKopman}
          removeKopman={removeKopman}
        />
        <ClassificationOverview data={data.classifications} />
      </div>
      <Modal
        open={showProfiles}
        modalContents={<StageProfiles raceId={raceId} stageNr={stagenr} />}
        closeFn={() => setShowProfiles(false)}
        title="Profielen"
      />
    </div>
  );
};

export default StageSelection;
