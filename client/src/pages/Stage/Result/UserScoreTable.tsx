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

const RankChange = ({ change }: { change: number }) => {
  if (change === 0) return <>-</>;
  const dir = change > 0 ? "up" : "down";
  return (
    <span className={`rank-change-${dir}`}>
      {change > 0 ? "▲" : "▼"}
      {Math.abs(change)}
    </span>
  );
};

const centered = { style: { justifyContent: "center" } };

const columns: TableColumn<UserScore>[] = [
  { name: "", width: "30px", ...centered, selector: (_r, i) => (i ?? 0) + 1 },
  { name: "", width: "35px", ...centered, cell: (r) => <RankChange change={r.change} /> },
  { name: "Naam", grow: 2, selector: (r) => r.username },
  { name: "Stage Score", selector: (r) => r.stagescore },
  { name: "Total Score", selector: (r) => r.totalscore },
];

const rowStyles = [{ when: (r: UserScore) => r.isLoggedInUser, classNames: ["selected"] }];

const UserScoreTable = ({ data }: { data: UserScore[] }) => (
  <SreDataTable
    columns={columns}
    data={data}
    pointerOnHover
    pagination
    paginationPerPage={20}
    conditionalRowStyles={rowStyles}
  />
);

export default UserScoreTable;
