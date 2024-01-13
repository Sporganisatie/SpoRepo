import { z } from "zod";
import { riderScoreSchema } from "../Result/TeamResultsTable";
import { userScoreSchema } from "../Result/UserScoreTable";
import { classificationsSchema } from "./StageSelectionData";

export const stageResultDataSchema = z.object({
  userScores: z.array(userScoreSchema),
  teamResult: z.array(riderScoreSchema),
  classifications: classificationsSchema,
});

export type StageResultData = z.infer<typeof stageResultDataSchema>;
