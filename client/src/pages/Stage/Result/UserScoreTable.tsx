import type { TableColumn } from "react-data-table-component";
import { z } from "zod";
import SreDataTable from "../../../components/shared/SreDataTable";

export const userScoreSchema = z.object({
  username: z.string(),
  stagescore: z.number(),
  totalscore: z.number(),
  change: z.number(),
  isLoggedInUser: z.boolean(),
});

export type UserScore = z.infer<typeof userScoreSchema>;

const formatRankChange = (change: number) => {
  if (change === 0) return "-";
  if (change > 0) return `▲${change}`;
  return `▼${Math.abs(change)}`;
};

const userScoreRowStyle = [
  {
    when: (row: UserScore) => row.isLoggedInUser,
    classNames: ["selected"],
  },
];

const UserScoreTable = ({ data }: { data: UserScore[] }) => {
  const columns: TableColumn<UserScore>[] = [
    {
      name: "",
      width: "30px",
      style: { justifyContent: "center", },
      selector: (_row: UserScore, rowIndex?: number) => (rowIndex ?? 0) + 1,
    },
    {
      name: "",
      width: "35px",
      style: { justifyContent: "center", },
      selector: (row: UserScore) => formatRankChange(row.change),
      conditionalCellStyles: [
        {
          when: (row) => row.change < 0,
          style: {
            color: "red",
          },
        },
        {
          when: (row) => row.change > 0,
          style: {
            color: "green",
          },
        },
      ],
    },
    {
      name: "Naam",
      grow: 2,
      selector: (row: UserScore) => row.username,
    },
    {
      name: "Stage Score",
      selector: (row: UserScore) => row.stagescore,
    },
    {
      name: "Total Score",
      selector: (row: UserScore) => row.totalscore,
    },
  ];

  return (
    <SreDataTable
      columns={columns}
      data={data}
      pointerOnHover
      pagination
      paginationPerPage={20}
      conditionalRowStyles={userScoreRowStyle}
      className="user-score-table" // Add specific class to override global styles
    />
  );
};

export default UserScoreTable;
