import type { StageComparisonRider } from "../../../models/UserSelection";
import { StageSelectedEnum } from "../../../models/UserSelection";
import RiderLink from "../RiderLink";
import Table from "../../ui/table/Table";

const rowClass = (row: StageComparisonRider) => {
  const base =
    row.selected === StageSelectedEnum.InStageSelection
      ? "selected"
      : row.selected === StageSelectedEnum.InTeam
        ? "notselected"
        : undefined;
  if (row.dnf) return base ? `${base} dnf` : "dnf";
  return base;
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
    noHead
    keyField={(r) => r.rider?.riderId ?? `total-${r.totalScore}`}
    rowClassName={rowClass}
  >
    {(col) => [
      col.text(
        (r) =>
          r.stagePos == null || r.stagePos === 0
            ? r.dnf
              ? "DNF"
              : ""
            : `${r.stagePos}e`,
        { width: "50px" },
      ),
      col.text(
        (r) =>
          r.rider ? (
            <RiderLink rider={r.rider} kopman={r.kopman} />
          ) : r.totalScore === -1 ? (
            ""
          ) : (
            "Totaal"
          ),
        { width: "200px" },
      ),
      col.text((r) => (r.totalScore === -1 ? "" : r.totalScore), { width: "60px" }),
    ]}
  </Table>
);

export default TeamComparisonTable;
