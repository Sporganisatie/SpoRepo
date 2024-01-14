import DataTable, { TableColumn } from "react-data-table-component";
import RiderLink from "../../../components/shared/RiderLink";
import { StageSelectedEnum } from "../../../models/UserSelection";
import { ClassificationRow } from "../models/StageSelectionData";

const classificationRowStyle = [
  {
    when: (row: ClassificationRow) =>
      row.selected === StageSelectedEnum.InStageSelection,
    classNames: ["selected"],
  },
  {
    when: (row: ClassificationRow) => row.selected === StageSelectedEnum.InTeam,
    classNames: ["notselected"],
  },
];

const ClassificationTable = ({
  rows,
  title,
  resultColName,
  pagination,
  showRankChange,
}: {
  rows: ClassificationRow[];
  title: string;
  resultColName: string;
  pagination?: boolean;
  showRankChange?: boolean;
}) => {
  const columns: TableColumn<ClassificationRow>[] = [
    {
      name: "",
      maxWidth: "100px",
      selector: (row: ClassificationRow) => row.result.position,
    },
    {
      name: "",
      width: "70px",
      selector: (row: ClassificationRow) => row.result.change ?? "",
      omit: !showRankChange,
      conditionalCellStyles: [
        {
          when: (row) => row.result.change?.startsWith("▼") ?? false,
          style: {
            color: "red",
          },
        },
        {
          when: (row) => row.result.change?.startsWith("▲") ?? false,
          style: {
            color: "green",
          },
        },
      ],
    },
    {
      name: "Naam",
      minWidth: "200px",
      cell: (row: ClassificationRow) => <RiderLink rider={row.rider} />,
    },
    {
      name: "Team",
      minWidth: "200px",
      cell: (row: ClassificationRow) => row.team,
    },
    {
      name: resultColName,
      selector: (row: ClassificationRow) => row.result.result,
    },
  ];

  return (
    <div style={{ borderStyle: "solid" }}>
      <DataTable
        title={title}
        columns={columns}
        data={rows}
        conditionalRowStyles={classificationRowStyle}
        striped
        highlightOnHover
        pointerOnHover
        dense
        pagination={pagination}
        paginationPerPage={20}
      />
    </div>
  );
};

export default ClassificationTable;
