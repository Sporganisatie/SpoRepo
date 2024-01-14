import StageResult from "./Result/StageResult";
import StageSelection from "./Selection/StageSelection";
import ArrowSelect from "../../components/ArrowSelect";
import { SelectOption } from "../../components/Select";
import { StageStateEnum, useStage } from "./StageHook";

const stages: SelectOption<string>[] = Array.from({ length: 22 }, (_, i) => ({
  displayValue: (i + 1).toString(),
  value: (i + 1).toString(),
}));

const Stage = () => {
  const { stagenr, stageState, setStage } = useStage();

  return (
    <div>
      <ArrowSelect
        value={stagenr}
        allowLooping={false}
        options={stages}
        onChange={(selectedValue) => {
          setStage(selectedValue);
        }}
      />
      {stageState === StageStateEnum.Selection && <StageSelection />}
      {stageState === StageStateEnum.Started && <StageResult />}
    </div>
  );
};

export default Stage;
