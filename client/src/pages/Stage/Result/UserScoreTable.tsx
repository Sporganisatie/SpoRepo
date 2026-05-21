import { z } from "zod";
import Table from "@/components/ui/table/Table";

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
    rowClassName={(r) => (r.isLoggedInUser ? "current-user" : undefined)}
  >
    {(col) => [
      col.text((_, i) => i + 1, { width: "30px", align: "center", padding: "0" }),
      col.rankChange((r) => r.change, { width: "35px", padding: "0" }),
      col.text((r) => r.username, { name: "Naam" }),
      col.text((r) => r.stagescore, { name: "Stage Score" }),
      col.text((r) => r.totalscore, { name: "Total Score" }),
    ]}
  </Table>
);

export default UserScoreTable;
