import type { Rider } from "../../../models/Rider";
import { StageSelectedEnum } from "../../../models/UserSelection";
import Table from "../../ui/table/Table";

export type AllSelectedRiderRow = {
  rider: Rider;
  count: number;
  users: string[];
  selected: StageSelectedEnum;
};

const rowClass = (row: AllSelectedRiderRow) => {
  if (row.selected === StageSelectedEnum.InStageSelection) return "selected";
  if (row.selected === StageSelectedEnum.InTeam) return "notselected";
  return undefined;
};

const AllSelectedRiders = ({ riders }: { riders: AllSelectedRiderRow[] }) => (
  <Table
    data={riders}
    title="Alle Geselecteerd"
    noHead
    keyField={(r) => r.rider.riderId}
    rowClassName={rowClass}
  >
    {(col) => [
      col.rider((r) => r.rider, { name: "Naam", width: "200px" }),
      col.text("#", (r) => r.count, { width: "60px" }),
      col.text("Users", (r) => [...r.users].sort().join(", "), { width: "310px" }),
    ]}
  </Table>
);

export default AllSelectedRiders;
