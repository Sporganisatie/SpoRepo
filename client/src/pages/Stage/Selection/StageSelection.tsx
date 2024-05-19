import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import ClassificationOverview from "./ClassificationOverview";
import { useStageSelection } from "./StageSelectionHook";
import { useStage } from "../StageHook";
import SelectionsComplete from "./SelectionComplete";

const options: Intl.DateTimeFormatOptions = {
  weekday: "long",
  day: "numeric",
  month: "long",
  hour: "numeric",
  minute: "numeric",
};

const StageSelection = () => {
  const { stagenr } = useStage();
  document.title = `Etappe ${stagenr} opstelling`;

  const { data, isLoading, addRider, removeRider, addKopman, removeKopman } =
    useStageSelection();

  let navigate = useNavigate();

  return (
    <div>
      {data ? (
        <div>
          <div style={{ margin: "10px 0" }}>
            Deadline:{" "}
            {data.deadline?.toLocaleDateString("nl-NL", options) ?? ""}
          </div>
          {stagenr === "1" && (
            <button onClick={() => navigate("/")}>Teamselectie</button>
          )}
          <SelectionsComplete
            compleet={data.compleet}
            budgetCompleet={data.budgetCompleet}
          />
          <div style={{ display: "flex" }}>
            <div style={{ margin: "0 5px" }}>
              <StageSelectionTeam
                team={data.team}
                isFetching={isLoading}
                addRider={addRider}
                removeRider={removeRider}
                addKopman={addKopman}
                removeKopman={removeKopman}
              />
            </div>
            <div style={{ margin: "0 5px" }}>
              <ClassificationOverview data={data.classifications} />
            </div>
          </div>
        </div>
      ) : (
        <></>
      )}
    </div>
  );
};

export default StageSelection;
