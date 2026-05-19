import type { TableColumn } from "react-data-table-component";
import RiderLink from "../../components/shared/RiderLink";
import { SelectableEnum } from "../../models/SelectableEnum";
import type { SelectableRider } from "./Models/SelectableRider";
import SelectableRiderFoldout from "./SelectableRiderFoldOut";
import SreDataTable from "../../components/shared/SreDataTable";

const conditionalRowStyles = [
  {
    when: (row: SelectableRider) => row.selectable !== SelectableEnum.Open,
    style: {
      backgroundColor: "#450a0a",
    },
  },
  {
    when: (row: SelectableRider) => row.selectable === SelectableEnum.Selected,
    style: {
      backgroundColor: "#64748b",
    },
  },
];

const SelectableRidersTable = ({
  data,
  loading,
  totalRiders,
  addRider,
  removeRider,
}: {
  data: SelectableRider[];
  loading: boolean;
  totalRiders: number;
  addRider: (id: number) => void;
  removeRider: (id: number) => void;
}) => {
  const columns: TableColumn<SelectableRider>[] = [
    {
      name: "Naam",
      width: "50",
      cell: (row: SelectableRider) => <RiderLink rider={row.details.rider} />,
    },
    {
      name: "Price",
      width: "100px",
      selector: (row: SelectableRider) => row.details.price,
    },
    {
      name: "Team",
      selector: (row: SelectableRider) => row.details.team,
    },
    {
      cell: (row: SelectableRider) => {
        switch (row.selectable) {
          case SelectableEnum.Open:
            return (
              <button
                className="teamselect-rider-button select"
                onClick={() => addRider(row.details.riderParticipationId)}
              >
                ➤
              </button>
            );
          case SelectableEnum.Selected:
            return (
              <button
                className="teamselect-rider-button deselect"
                onClick={() => removeRider(row.details.riderParticipationId)}
              >
                🞫
              </button>
            );
          default:
            return <></>;
        }
      },
    },
  ];

  return (
    <div className="panel rdt-action-cell-last">
      <div className="panel-header">
        <h3 className="panel-title">Beschikbare renners</h3>
        <span className="panel-meta">
          {data.length} van {totalRiders}
        </span>
      </div>
      <SreDataTable
        columns={columns}
        data={data}
        progressPending={loading}
        conditionalRowStyles={conditionalRowStyles}
        expandableRows
        expandableRowsComponent={SelectableRiderFoldout}
        expandOnRowClicked
        expandableRowsHideExpander
        pointerOnHover
      />
    </div>
  );
};

export default SelectableRidersTable;
