import { z } from "zod";
import { riderSchema } from "../../../models/Rider";
import { StageSelectedEnum } from "../../../models/UserSelection";
import { stageSelectableRiderSchema } from "./StageSelectableRider";

export const baseResultSchema = z.object({
  position: z.number(),
  score: z.number(),
  result: z.string(),
  selected: z.nativeEnum(StageSelectedEnum).optional(),
  change: z.string().nullable(),
});
export type BaseResult = z.infer<typeof baseResultSchema>;

export const classificationRow = z.object({
  rider: riderSchema,
  team: z.string(),
  result: baseResultSchema,
  selected: z.nativeEnum(StageSelectedEnum).optional(),
});
export type ClassificationRow = z.infer<typeof classificationRow>;

export const classificationsSchema = z.object({
  gc: z.array(classificationRow),
  points: z.array(classificationRow),
  kom: z.array(classificationRow),
  youth: z.array(classificationRow),
  stage: z
    .array(classificationRow)
    .nullable()
    .transform((s) => s ?? undefined),
});
export type Classifications = z.infer<typeof classificationsSchema>;

export const stageSelectionDataSchema = z.object({
  team: z.array(stageSelectableRiderSchema),
  deadline: z
    .string()
    .nullable()
    .transform((dateString) => {
      if (!dateString) {
        return;
      }
      const deadline = new Date(dateString);
      deadline.setHours(deadline.getHours() + 2);
      return deadline;
    }),
  classifications: classificationsSchema,
  compleet: z
    .number()
    .optional()
    .transform((val) => val ?? 0),
  budgetCompleet: z.number().nullable(),
});
export type StageSelectionData = z.infer<typeof stageSelectionDataSchema>;
