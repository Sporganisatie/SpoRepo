import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faShirt } from "@fortawesome/free-solid-svg-icons";

export interface BarValue {
  selected: number;
  kopman: boolean;
}

interface SelectionCompleteProps {
  bars: BarValue[];
  jerseyClass: string;
}

const Bar = ({ selected, kopman, jerseyClass }: BarValue & { jerseyClass: string }) => {
  const clamped = Math.max(0, Math.min(9, selected));
  const pct = (clamped / 9) * 100;
  const complete = clamped === 9 && kopman;

  return (
    <div className="ss-completion-row">
      <div className="ss-completion-track">
        <div
          className={`ss-completion-fill ${complete ? "complete" : ""}`}
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
