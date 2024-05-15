import { z } from "zod";

export const riderSchema = z.object({
  firstname: z.string(),
  lastname: z.string(),
  initials: z.string(),
  country: z.string(),
  riderId: z.number(),
});
export type Rider = z.infer<typeof riderSchema>;
