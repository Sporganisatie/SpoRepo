import { z } from "zod";
import { riderSchema } from "./Rider";

export const riderParticipationSchema = z.object({
  riderParticipationId: z.number(),
  raceId: z.number(),
  riderId: z.number(),
  price: z.number(),
  dnf: z.boolean(),
  team: z.string(),
  punch: z.number(),
  climb: z.number(),
  tt: z.number(),
  sprint: z.number(),
  gc: z.number(),
  type: z.string(),
  rider: riderSchema,
});
export type RiderParticipation = z.infer<typeof riderParticipationSchema>;
