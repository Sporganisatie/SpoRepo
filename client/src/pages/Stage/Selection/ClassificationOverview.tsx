import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faChevronDown, faChevronUp } from "@fortawesome/free-solid-svg-icons";
import ClassificationTable from "./ClassificationTable";
import type { Classifications, ClassificationRow } from "@/pages/Stage/models/StageSelectionData";

const COLLAPSED_COUNT = 5;

interface PanelProps {
  title: string;
  rows: ClassificationRow[];
}

const ClassificationPanel = ({ title, rows }: PanelProps) => {
  const [expanded, setExpanded] = useState(false);
  const visibleRows = expanded ? rows : rows.slice(0, COLLAPSED_COUNT);
  const canExpand = rows.length > COLLAPSED_COUNT;

  return (
    <div className="panel">
      <div className="panel-header">
        <h3 className="panel-title">{title}</h3>
        {canExpand && (
          <button
            className="panel-toggle"
            onClick={() => setExpanded((e) => !e)}
            title={expanded ? "Minder tonen" : `Toon nog ${rows.length - COLLAPSED_COUNT}`}
          >
            <FontAwesomeIcon icon={expanded ? faChevronUp : faChevronDown} />
          </button>
        )}
      </div>
      <ClassificationTable rows={visibleRows} />
    </div>
  );
};

const ClassificationOverview = ({ data }: { data: Classifications }) => {
  return (
    <div className="stage-select-classifications">
      <ClassificationPanel title="Algemeen" rows={data.gc} />
      <ClassificationPanel title="Punten" rows={data.points} />
      <ClassificationPanel title="Berg" rows={data.kom} />
      <ClassificationPanel title="Jongeren" rows={data.youth} />
    </div>
  );
};

export default ClassificationOverview;
