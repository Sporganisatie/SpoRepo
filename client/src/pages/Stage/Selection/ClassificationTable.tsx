import { StageSelectedEnum } from "../../../models/UserSelection";
import type { ClassificationRow } from "../models/StageSelectionData";
import Table from "../../../components/ui/table/Table";

const rowClass = (row: ClassificationRow) => {
  if (row.selected === StageSelectedEnum.InStageSelection) return "selected";
  if (row.selected === StageSelectedEnum.InTeam) return "notselected";
  return undefined;
};

const ClassificationTable = ({
  rows,
  resultColName,
  pagination,
  showRankChange,
}: {
  rows: ClassificationRow[];
  resultColName: string;
  pagination?: boolean;
  showRankChange?: boolean;
}) => (
  <Table
    data={rows}
    keyField={(r) => r.rider.riderId}
    paginated={pagination}
    noHead
    rowClassName={rowClass}
  >
    {(col) => {
      const cols = [
        col.position((r) => r.result.position, {
          name: "",
          width: "30px",
          align: "center",
          padding: "0",
        }),
      ];
      if (showRankChange) {
        cols.push(col.rankChange((r) => r.result.change, { width: "40px", padding: "0" }));
      }
      cols.push(
        col.rider((r) => r.rider, { name: "Naam", width: "38%" }),
        col.text("Team", (r) => <span className="rider-team-text">{r.team}</span>, {
          width: "38%",
        }),
        col.text(resultColName, (r) => r.result.result),
      );
      return cols;
    }}
  </Table>
);

export default ClassificationTable;
