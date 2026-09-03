import { z } from "zod";

export const riderStageScoreRowSchema = z.object({
    stagenr: z.number().nullable(),
    label: z.string(),
    stageScore: z.number().nullable(),
    gcScore: z.number(),
    pointsScore: z.number(),
    komScore: z.number(),
    youthScore: z.number(),
    teamScore: z.number(),
    totalScore: z.number(),
});
export type RiderStageScoreRow = z.infer<typeof riderStageScoreRowSchema>;

export const classificationCellSchema = z.object({
    position: z.number().nullable(),
    result: z.string(),
});
export type ClassificationCell = z.infer<typeof classificationCellSchema>;

export const riderClassificationRowSchema = z.object({
    stagenr: z.number().nullable(),
    label: z.string(),
    stagePos: z.number().nullable(),
    stageResult: z.string(),
    gc: classificationCellSchema,
    points: classificationCellSchema,
    kom: classificationCellSchema,
    youth: classificationCellSchema,
});
export type RiderClassificationRow = z.infer<typeof riderClassificationRowSchema>;

export const riderSelectionRowSchema = z.object({
    username: z.string(),
    points: z.number(),
    timesSelected: z.number(),
    timesKopman: z.number(),
    isLoggedInUser: z.boolean(),
});
export type RiderSelectionRow = z.infer<typeof riderSelectionRowSchema>;

export const riderRaceDetailSchema = z.object({
    firstname: z.string(),
    lastname: z.string(),
    tooExpensive: z.boolean(),
    active: z.boolean(),
    dnf: z.boolean(),
    price: z.number(),
    totalParticipants: z.number(),
    timesSelectedByAnyone: z.number(),
    stages: z.array(riderStageScoreRowSchema),
    classifications: z.array(riderClassificationRowSchema),
    selections: z.array(riderSelectionRowSchema),
});
export type RiderRaceDetail = z.infer<typeof riderRaceDetailSchema>;

