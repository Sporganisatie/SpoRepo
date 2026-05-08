import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import ClassificationOverview from "./ClassificationOverview";
import { useStageSelection } from "./StageSelectionHook";
import { useStage } from "../StageHook";
import StageNav from "../StageNav";
import CountdownClock24H, { isWithin24h } from "../../teamselection/CountdownClock";
import "./StageSelection.css";

const dateOptions: Intl.DateTimeFormatOptions = {
  weekday: "long",
  day: "numeric",
  month: "long",
  hour: "numeric",
  minute: "numeric",
};

const StageSelection = () => {
  const { stagenr } = useStage();
  document.title = `Etappe ${stagenr} opstelling`;

  const { data, isLoading, addRider, removeRider, addKopman, removeKopman } = useStageSelection();
  const navigate = useNavigate();

  if (!data) {
    return <div className="stage-selection-page" />;
  }

  const within24h = data.deadline != null && isWithin24h(data.deadline);

  return (
    <div className="stage-selection-page">
      <div className="ss-page-header">
        <StageNav />
        {!within24h && (
          <div className="ss-deadline">
            {data.deadline?.toLocaleDateString("nl-NL", dateOptions) ?? ""}
          </div>
        )}
        <div className="ss-page-header-right">
          {within24h && data.deadline && (
            <CountdownClock24H targetDate={data.deadline} className="compact" />
          )}
          {stagenr === "1" && (
            <button className="ss-page-cta" onClick={() => navigate("/")}>
              Teamselectie
            </button>
          )}
        </div>
      </div>

      <div className="ss-body">
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
    </div>
  );
};

export default StageSelection;
