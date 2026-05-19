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

const SEGMENTS = 9;
const SVG_WIDTH = 200;
const SVG_HEIGHT = 8;
const SVG_GAP = 1;
const SEG_W = (SVG_WIDTH - SVG_GAP * (SEGMENTS - 1)) / SEGMENTS;

const Bar = ({ selected, kopman, jerseyClass }: BarValue & { jerseyClass: string }) => {
  const clamped = clamp(selected, 0, SEGMENTS);
  const complete = clamped === SEGMENTS && kopman;
  const fillClass = complete ? "complete" : "danger";

  return (
    <div className="stage-select-completion-row">
      <svg
        className="meter-svg segmented"
        viewBox={`0 0 ${SVG_WIDTH} ${SVG_HEIGHT}`}
        preserveAspectRatio="none"
        aria-hidden
      >
        {Array.from({ length: SEGMENTS }, (_, i) => {
          const filled = i < clamped;
          return (
            <rect
              key={i}
              className={`meter-segment ${filled ? `filled ${fillClass}` : ""}`}
              x={i * (SEG_W + SVG_GAP)}
              y={0}
              width={SEG_W}
              height={SVG_HEIGHT}
            />
          );
        })}
      </svg>
      <FontAwesomeIcon
        icon={faShirt}
        className={`stage-select-completion-jersey ${kopman ? `active ${jerseyClass}` : ""}`}
      />
    </div>
  );
};

const SelectionsComplete = ({ bars, jerseyClass }: SelectionCompleteProps) => {
  return (
    <div className="stage-select-completion">
      {bars.map((b, i) => (
        <Bar key={i} selected={b.selected} kopman={b.kopman} jerseyClass={jerseyClass} />
      ))}
    </div>
  );
};

export default SelectionsComplete;
