import { useEffect, useState } from "react";
import ClassificationTable from "../Selection/ClassificationTable";
import type { Classifications } from "../models/StageSelectionData";
import "./StageClassifications.css";

type TabKey = "Etappe" | "Algemeen" | "Punten" | "Berg" | "Jongeren";

interface TabConfig {
  key: TabKey;
  resultColName: string;
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

  const tabs: TabConfig[] = [
    ...(finalStandings ? [] : ([{ key: "Etappe", resultColName: "Tijd" }] as TabConfig[])),
    { key: "Algemeen", resultColName: "Tijd" },
    { key: "Punten", resultColName: "Punten" },
    { key: "Berg", resultColName: "Punten" },
    { key: "Jongeren", resultColName: "Tijd" },
  ];

  const rowsForTab = (key: TabKey) => {
    switch (key) {
      case "Etappe":
        return data.stage ?? [];
      case "Algemeen":
        return data.gc;
      case "Punten":
        return data.points;
      case "Berg":
        return data.kom;
      case "Jongeren":
        return data.youth;
    }
  };

  const activeRows = rowsForTab(activeTab);
  const activeConfig = tabs.find((t) => t.key === activeTab) ?? tabs[0];

  return (
    <div className="ts-panel">
      <div className="ts-panel-header sr-tabs">
        {tabs.map(({ key }) => {
          const disabled = (rowsForTab(key)?.length ?? 0) === 0;
          return (
            <button
              key={key}
              className={`sr-tab ${activeTab === key ? "active" : ""}`}
              disabled={disabled}
              onClick={() => setActiveTab(key)}
            >
              {key}
            </button>
          );
        })}
      </div>
      <ClassificationTable
        rows={activeRows}
        resultColName={activeConfig.resultColName}
        pagination={true}
        showRankChange={activeTab !== "Etappe"}
      />
    </div>
  );
};

export default StageClassifications;
