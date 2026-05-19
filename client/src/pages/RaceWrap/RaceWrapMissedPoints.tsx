import { useEffect, useState } from "react";
import SreDataTable from "../../components/shared/SreDataTable";
import type { MissedPointsTotalRow } from "./RaceWrap";

const RaceWrapMissedPoints = ({ missedPoints }: { missedPoints: MissedPointsTotalRow[] }) => {
  const [missingPointsRanking, setMissingPointsRanking] = useState<MissedPointsTotalRow[]>([]);
  const [optimalScoreRanking, setOptimalScoreRanking] = useState<MissedPointsTotalRow[]>([]);

  useEffect(() => {
    setMissingPointsRanking(
      missedPoints
        .slice()
        .sort((a: MissedPointsTotalRow, b: MissedPointsTotalRow) => a.gemist - b.gemist)
    );
    setOptimalScoreRanking(
      missedPoints
        .slice()
        .sort((a: MissedPointsTotalRow, b: MissedPointsTotalRow) => b.optimaal - a.optimaal)
    );
  }, [missedPoints]);

  return (
    <div className="h-stack">
      <SreDataTable
          title="Gemiste punten"
          columns={[
            {
              name: "Naam",
              selector: (row: MissedPointsTotalRow) => row.username,
              sortable: true,
            },
            {
              name: "Gemist",
              selector: (row: MissedPointsTotalRow) => row.gemist,
              sortable: true,
            },
            {
              name: "Percentage",
              selector: (row: MissedPointsTotalRow) =>
                Math.round((row.gemist / row.optimaal) * 1000) / 10,
              sortable: true,
            },
          ]}
          data={missingPointsRanking}
        />
        <SreDataTable
          title="Optimale score"
          columns={[
            {
              name: "",
              width: "70px",
              selector: (row: MissedPointsTotalRow) => {
                if (row.optimalIndex < row.scoreIndex) {
                  return "▲" + (row.scoreIndex - row.optimalIndex);
                }
                if (row.optimalIndex > row.scoreIndex) {
                  return "▼" + (row.optimalIndex - row.scoreIndex);
                }
                return "";
              },
              conditionalCellStyles: [
                {
                  when: (row) => row.optimalIndex > row.scoreIndex,
                  style: {
                    color: "red",
                  },
                },
                {
                  when: (row) => row.optimalIndex < row.scoreIndex,
                  style: {
                    color: "green",
                  },
                },
              ],
            },
            {
              name: "Naam",
              selector: (row: MissedPointsTotalRow) => row.username,
              sortable: true,
            },
            {
              name: "Optimaal",
              selector: (row: MissedPointsTotalRow) => row.optimaal,
              sortable: true,
            },
          ]}
          data={optimalScoreRanking}
        />
    </div>
  );
};

export default RaceWrapMissedPoints;
