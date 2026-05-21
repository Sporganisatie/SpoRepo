import { useEffect, useMemo, useState } from "react";
import ClassificationTable from "@/pages/Stage/Selection/ClassificationTable";
import type { Classifications, ClassificationRow } from "@/pages/Stage/models/StageSelectionData";
import "./StageClassifications.css";

type TabKey = "Etappe" | "Algemeen" | "Punten" | "Berg" | "Jongeren";

interface TabConfig {
  rows: ClassificationRow[];
  showRankChange: boolean;
}

const StageClassifications = ({
  data,
  finalStandings,
}: {
  data: Classifications;
  finalStandings: boolean;
}) => {
  const [activeTab, setActiveTab] = useState<TabKey>("Etappe");

  useEffect(() => {
    setActiveTab(finalStandings ? "Algemeen" : "Etappe");
  }, [finalStandings]);

  const tabs = useMemo(() => {
    const all: Array<[TabKey, TabConfig]> = [
      ["Etappe", { rows: data.stage ?? [], showRankChange: false }],
      ["Algemeen", { rows: data.gc, showRankChange: true }],
      ["Punten", { rows: data.points, showRankChange: true }],
      ["Berg", { rows: data.kom, showRankChange: true }],
      ["Jongeren", { rows: data.youth, showRankChange: true }],
    ];
    return all.filter(([key]) => !(finalStandings && key === "Etappe"));
  }, [data, finalStandings]);

  const active = tabs.find(([key]) => key === activeTab)?.[1] ?? tabs[0][1];

  return (
    <div className="panel">
      <div className="panel-header stage-result-tabs">
        {tabs.map(([key, cfg]) => (
          <button
            key={key}
            className={`stage-result-tab ${activeTab === key ? "active" : ""}`}
            disabled={cfg.rows.length === 0}
            onClick={() => setActiveTab(key)}
          >
            {key}
          </button>
        ))}
      </div>
      <ClassificationTable
        rows={active.rows}
        pagination={true}
        showRankChange={active.showRankChange}
      />
    </div>
  );
};

export default StageClassifications;
