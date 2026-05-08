import StageResult from "./Result/StageResult";
import StageSelection from "./Selection/StageSelection";
import { StageStateEnum, useStage } from "./StageHook";

const Stage = () => {
  const { stageState } = useStage();

  return (
    <div>
      {stageState === StageStateEnum.Selection && <StageSelection />}
      {stageState === StageStateEnum.Started && <StageResult />}
    </div>
  );
};

export default Stage;
