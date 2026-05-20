import { z } from "zod";
import Table from "../../../components/ui/table/Table";

export const userScoreSchema = z.object({
  username: z.string(),
  stagescore: z.number(),
  totalscore: z.number(),
  change: z.number(),
  isLoggedInUser: z.boolean(),
});

export type UserScore = z.infer<typeof userScoreSchema>;

const UserScoreTable = ({ data }: { data: UserScore[] }) => (
  <Table
    data={data}
    keyField="username"
    paginated
    rowClassName={(r) => (r.isLoggedInUser ? "selected" : undefined)}
  >
    {(col) => [
      col.position((_r, i) => i + 1, { name: "", width: "30px", align: "center", padding: "0" }),
      col.rankChange((r) => r.change, { width: "35px", padding: "0" }),
      col.text("Naam", (r) => r.username),
      col.text("Stage Score", (r) => r.stagescore),
      col.text("Total Score", (r) => r.totalscore),
    ]}
  </Table>
);

export default UserScoreTable;
