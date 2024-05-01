import { z } from "zod";
import { riderParticipationSchema } from "../../../models/RiderParticipation";

export const stageSelectableRiderSchema = z.object({
  rider: riderParticipationSchema,
  selected: z.boolean(),
  isKopman: z.boolean(),
});
export type StageSelectableRider = z.infer<typeof stageSelectableRiderSchema>;
