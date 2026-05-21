import type { StageComparisonRider } from "@/models/UserSelection";
import { stageSelectionRowClass } from "@/models/UserSelection";
import RiderLink from "@/components/shared/RiderLink";
import Table from "@/components/ui/table/Table";

const rowClass = (row: StageComparisonRider) => {
  const parts: string[] = [];
  const selection = stageSelectionRowClass(row.selected);
  if (selection) parts.push(selection);
  if (row.dnf) parts.push("dnf");
  return parts.length ? parts.join(" ") : undefined;
};

const renderStagePos = (r: StageComparisonRider) => {
  if (r.stagePos != null && r.stagePos !== 0) return `${r.stagePos}e`;
  if (r.dnf) return "DNF";
  return "";
};

const renderRider = (r: StageComparisonRider) => {
  if (r.rider) return <RiderLink rider={r.rider} kopman={r.kopman} />;
  if (r.totalScore === -1) return "";
  return "Totaal";
};

const renderTotalScore = (r: StageComparisonRider) => {
  if (r.totalScore === -1) return "";
  return r.totalScore;
};

const TeamComparisonTable = ({
  title,
  riders,
}: {
  title: string;
  riders: StageComparisonRider[];
}) => (
  <Table
    data={riders}
    title={title}
    hideHeader
    rowKey={(r) => r.rider?.riderId ?? `total-${r.totalScore}`}
    rowClassName={rowClass}
  >
    {(col) => [
      col.text(renderStagePos, { width: "50px" }),
      col.text(renderRider, { width: "200px" }),
      col.text(renderTotalScore, { width: "60px" }),
    ]}
  </Table>
);

export default TeamComparisonTable;
