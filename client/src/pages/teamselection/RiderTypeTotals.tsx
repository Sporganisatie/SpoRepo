import DataTable, { TableColumn } from "react-data-table-component";
import { RiderParticipation } from "../../models/RiderParticipation";

export interface RiderTypeTotalRow {
  rowName: string;
  klassement: string;
  klimmer: string;
  sprinter: string;
  tijdrijder: string;
  aanvaller: string;
  knecht: string;
}

function sumType(team: RiderParticipation[], typeName: string): string {
  return (
    team
      .filter((r) => r.type === typeName)
      .reduce((sum, current) => sum + current.price / 1000000, 0) + "M"
  );
}

function countType(team: RiderParticipation[], typeName: string): string {
  return team.filter((r) => r.type === typeName).length.toString();
}

const RiderTypeTotals = ({ team }: { team: RiderParticipation[] }) => {
  var totals: Array<RiderTypeTotalRow> = [
    {
      rowName: "Budget",
      klassement: sumType(team, "Klassement"),
      klimmer: sumType(team, "Klimmer"),
      sprinter: sumType(team, "Sprinter"),
      tijdrijder: sumType(team, "Tijdrijder"),
      aanvaller: sumType(team, "Aanvaller"),
      knecht: sumType(team, "Knecht"),
    },
    {
      rowName: "Aantal",
      klassement: countType(team, "Klassement"),
      klimmer: countType(team, "Klimmer"),
      sprinter: countType(team, "Sprinter"),
      tijdrijder: countType(team, "Tijdrijder"),
      aanvaller: countType(team, "Aanvaller"),
      knecht: countType(team, "Knecht"),
    },
  ];

  const columns: TableColumn<RiderTypeTotalRow>[] = [
    {
      name: " ",
      cell: (row: RiderTypeTotalRow) => row.rowName,
    },
    {
      name: "Klassement",
      cell: (row: RiderTypeTotalRow) => row.klassement,
    },
    {
      name: "Klimmer",
      cell: (row: RiderTypeTotalRow) => row.klimmer,
    },
    {
      name: "Sprinter",
      selector: (row: RiderTypeTotalRow) => row.sprinter,
    },
    {
      name: "Tijdrijder",
      selector: (row: RiderTypeTotalRow) => row.tijdrijder,
    },
    {
      name: "Aanvaller",
      selector: (row: RiderTypeTotalRow) => row.aanvaller,
    },
    {
      name: "Knecht",
      selector: (row: RiderTypeTotalRow) => row.knecht,
    },
  ];

  return (
    <div style={{ width: "100%" }}>
      <DataTable
        title={`Totaal per type`}
        columns={columns}
        data={totals}
        striped
        pointerOnHover
        dense
        theme="dark"
      />
    </div>
  );
};

export default RiderTypeTotals;
