import { SelectableEnum } from "../../models/SelectableEnum";
import type { SelectableRider } from "./Models/SelectableRider";
import SelectableRiderFoldout from "./SelectableRiderFoldOut";
import Table from "../../components/ui/table/Table";

const rowClass = (row: SelectableRider) => {
  if (row.selectable === SelectableEnum.Selected) return "row-in-team";
  if (row.selectable !== SelectableEnum.Open) return "row-blocked";
  return undefined;
};

const SelectableRidersTable = ({
  data,
  totalRiders,
  addRider,
  removeRider,
}: {
  data: SelectableRider[];
  totalRiders: number;
  addRider: (id: number) => void;
  removeRider: (id: number) => void;
}) => (
  <div className="panel">
    <div className="panel-header">
      <h3 className="panel-title">Beschikbare renners</h3>
      <span className="panel-meta">
        {data.length} van {totalRiders}
      </span>
    </div>
    <Table
      data={data}
      pointerOnHover
      rowKey={(r) => r.details.riderParticipationId}
      rowClassName={rowClass}
      expandedContent={(r) => <SelectableRiderFoldout data={r} />}
    >
      {(col) => [
        col.rider((r) => r.details.rider, { name: "Naam", width: "50%" }),
        col.text((r) => r.details.price, { name: "Price", width: "100px" }),
        col.text((r) => r.details.team, { name: "Team" }),
        col.text(
          (r) => {
            switch (r.selectable) {
              case SelectableEnum.Open:
                return (
                  <button
                    className="teamselect-rider-button select"
                    onClick={() => addRider(r.details.riderParticipationId)}
                  >
                    ➤
                  </button>
                );
              case SelectableEnum.Selected:
                return (
                  <button
                    className="teamselect-rider-button deselect"
                    onClick={() => removeRider(r.details.riderParticipationId)}
                  >
                    🞫
                  </button>
                );
              default:
                return null;
            }
          },
          { width: "2.7rem", padding: "0" },
        ),
      ]}
    </Table>
  </div>
);

export default SelectableRidersTable;
