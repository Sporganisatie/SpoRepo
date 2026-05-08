import { useMemo } from "react";
import ArrowSelect from "../../components/ArrowSelect";
import type { SelectOption } from "../../components/Select";
import { useMediaQuery } from "../../components/shared/useMediaQuery";
import { useStage } from "./StageHook";

const StageNav = () => {
  const { stagenr, setStage } = useStage();
  const isMobile = useMediaQuery("(max-width: 600px)");

  const stages: SelectOption<string>[] = useMemo(
    () =>
      Array.from({ length: 22 }, (_, i) => ({
        displayValue: isMobile ? String(i + 1) : `Etappe ${i + 1}`,
        value: (i + 1).toString(),
      })),
    [isMobile]
  );

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
