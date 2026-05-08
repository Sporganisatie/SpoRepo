import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faChevronDown, faChevronUp } from "@fortawesome/free-solid-svg-icons";
import ClassificationTable from "./ClassificationTable";
import type { Classifications, ClassificationRow } from "../models/StageSelectionData";

const COLLAPSED_COUNT = 5;

interface PanelProps {
  title: string;
  rows: ClassificationRow[];
  resultColName: string;
}

const ClassificationPanel = ({ title, rows, resultColName }: PanelProps) => {
  const [expanded, setExpanded] = useState(false);
  const visibleRows = expanded ? rows : rows.slice(0, COLLAPSED_COUNT);
  const canExpand = rows.length > COLLAPSED_COUNT;

  return (
    <div className="ts-panel">
      <div className="ts-panel-header">
        <h3 className="ts-panel-title">{title}</h3>
        {canExpand && (
          <button
            className="ts-panel-toggle"
            onClick={() => setExpanded((e) => !e)}
            title={expanded ? "Minder tonen" : `Toon nog ${rows.length - COLLAPSED_COUNT}`}
            aria-expanded={expanded}
          >
            <FontAwesomeIcon icon={expanded ? faChevronUp : faChevronDown} />
          </button>
        )}
      </div>
      <ClassificationTable rows={visibleRows} resultColName={resultColName} />
    </div>
  );
};

const ClassificationOverview = ({ data }: { data: Classifications }) => {
  return (
    <div className="ss-classifications">
      <ClassificationPanel title="Algemeen" rows={data.gc} resultColName="Tijd" />
      <ClassificationPanel title="Punten" rows={data.points} resultColName="Punten" />
      <ClassificationPanel title="Berg" rows={data.kom} resultColName="Punten" />
      <ClassificationPanel title="Jongeren" rows={data.youth} resultColName="Tijd" />
    </div>
  );
};

export default ClassificationOverview;
