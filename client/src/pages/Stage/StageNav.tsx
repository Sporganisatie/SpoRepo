import ArrowSelect from "../../components/ArrowSelect";
import type { SelectOption } from "../../components/Select";
import { useStage } from "./StageHook";

const stages: SelectOption<string>[] = Array.from({ length: 22 }, (_, i) => ({
  displayValue: `Etappe ${i + 1}`,
  value: (i + 1).toString(),
}));

const StageNav = () => {
  const { stagenr, setStage } = useStage();
  return (
    <ArrowSelect
      value={stagenr}
      allowLooping={false}
      options={stages}
      onChange={(selectedValue) => setStage(selectedValue)}
    />
  );
};

export default StageNav;
