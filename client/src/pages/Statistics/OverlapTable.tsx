import type { TableColumn } from "react-data-table-component";
import type { OverlapRow } from "./Overlap";
import SreDataTable from "../../components/shared/SreDataTable";

const OverlapTable = ({ overlaps, title }: { overlaps: OverlapRow[]; title: string }) => {
  const columns: TableColumn<OverlapRow>[] = [
    {
      name: "User",
      width: "100px",
      cell: (row: OverlapRow) => row.user,
    },
  ];

  if (overlaps[0] !== undefined) {
    for (const key in overlaps[0].overlaps) {
      columns.push({
        name: key,
        width: "80px",
        cell: (row: OverlapRow) => (row.overlaps[key] === -1 ? "X" : row.overlaps[key]),
      });
    }
  }

  const maxwidth = columns.length * 80 + 20;

  return <SreDataTable title={title} columns={columns} data={overlaps} maxwidth={maxwidth} />;
};

export default OverlapTable;
