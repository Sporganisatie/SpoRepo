import StageSelectionTeam from "./StageSelectionTeam";
import { useNavigate } from "react-router-dom";
import ClassificationOverview from "./ClassificationOverview";
import { StageSelectionHook, useStageSelection } from "./StageSelectionHook";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

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

  const { data, isFetching, addRider, removeRider, addKopman, removeKopman } =
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
          {<button onClick={() => navigate("/")}>Teamselectie</button>}
          <div style={{ display: "flex" }}>
            <div style={{ margin: "0 5px" }}>
              <StageSelectionTeam
                team={data.team}
                isFetching={isFetching}
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
