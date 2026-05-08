import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faShirt } from "@fortawesome/free-solid-svg-icons";
import { clamp } from "../../../lib/math";

export interface BarValue {
  selected: number;
  kopman: boolean;
}

interface SelectionCompleteProps {
  bars: BarValue[];
  jerseyClass: string;
}

const Bar = ({ selected, kopman, jerseyClass }: BarValue & { jerseyClass: string }) => {
  const clamped = clamp(selected, 0, 9);
  const pct = (clamped / 9) * 100;
  const complete = clamped === 9 && kopman;

  return (
    <div className="ss-completion-row">
      <div className="meter-track">
        <div
          className={`meter-fill ${complete ? "complete" : "danger"}`}
          style={{ width: `${pct}%` }}
        />
      </div>
      <FontAwesomeIcon
        icon={faShirt}
        className={`ss-completion-jersey ${kopman ? `active ${jerseyClass}` : ""}`}
      />
    </div>
  );
};

const SelectionsComplete = ({ bars, jerseyClass }: SelectionCompleteProps) => {
  return (
    <div className="ss-completion">
      {bars.map((b, i) => (
        <Bar key={i} selected={b.selected} kopman={b.kopman} jerseyClass={jerseyClass} />
      ))}
    </div>
  );
};

export default SelectionsComplete;
