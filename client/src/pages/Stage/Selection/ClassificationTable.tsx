import { stageSelectionRowClass } from "@/models/UserSelection";
import type { ClassificationRow } from "@/pages/Stage/models/StageSelectionData";
import Table from "@/components/ui/table/Table";

const ClassificationTable = ({
  rows,
  pagination,
  showRankChange,
}: {
  rows: ClassificationRow[];
  pagination?: boolean;
  showRankChange?: boolean;
}) => (
  <Table
    data={rows}
    rowKey={r => r.rider.riderId}
    paginated={pagination}
    hideHeader
    rowClassName={r => stageSelectionRowClass(r.selected)}
  >
    {(col) => [
      col.text(r => r.result.position || "", {
        width: "30px",
        align: "center",
        padding: "0",
      }),
      col.rankChange(r => r.result.change, {
        width: "40px",
        padding: "0",
        omit: !showRankChange,
      }),
      col.rider(r => r.rider, { width: "38%" }),
      col.text(r => <span className="rider-team-text">{r.team}</span>, { width: "38%" }),
      col.text(r => r.result.result),
    ]}
  </Table>
);

export default ClassificationTable;
