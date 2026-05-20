import { riderSchema } from "../../../models/Rider";
import { z } from "zod";
import Table from "../../../components/ui/table/Table";

export const riderScoreSchema = z.object({
  rider: riderSchema.nullable(),
  kopman: z.boolean(),
  stageScore: z.number(),
  stagePos: z.number().nullable(),
  classificationScore: z.number(),
  teamScore: z.number(),
  totalScore: z.number(),
});

export type RiderScore = z.infer<typeof riderScoreSchema>;

const TeamResultsTable = ({ data }: { data: RiderScore[] }) => (
  <Table data={data}>
    {(col) => [
      col.position((r) => r.stagePos, { width: "12%", ordinal: true }),
      col.rider((r) => r.rider, { kopman: (r) => r.kopman, fallback: "Totaal", width: "35%" }),
      col.text((r) => r.stageScore, { name: "Dag", width: "12%" }),
      col.text((r) => r.classificationScore, { name: "Klassementen", width: "17%" }),
      col.text((r) => r.teamScore, { name: "Team", width: "12%" }),
      col.text((r) => r.totalScore, { name: "Totaal", width: "12%" }),
    ]}
  </Table>
);

export default TeamResultsTable;
