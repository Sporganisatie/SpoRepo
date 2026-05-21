import type { Rider } from "@/models/Rider";
import { StageSelectedEnum, stageSelectionRowClass } from "@/models/UserSelection";
import Table from "@/components/ui/table/Table";

export type AllSelectedRiderRow = {
  rider: Rider;
  count: number;
  users: string[];
  selected: StageSelectedEnum;
};

const AllSelectedRiders = ({ riders }: { riders: AllSelectedRiderRow[] }) => (
  <Table
    data={riders}
    title="Alle Geselecteerd"
    hideHeader
    keyField={(r) => r.rider.riderId}
    rowClassName={(r) => stageSelectionRowClass(r.selected)}
  >
    {(col) => [
      col.rider((r) => r.rider, { width: "200px" }),
      col.text((r) => r.count, { width: "60px" }),
      col.text((r) => [...r.users].sort().join(", "), { width: "310px" }),
    ]}
  </Table>
);

export default AllSelectedRiders;
