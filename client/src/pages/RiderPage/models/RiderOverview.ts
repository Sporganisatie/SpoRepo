import { z } from "zod";

export const riderRaceOverviewRowSchema = z.object({
    raceId: z.number(),
    raceName: z.string(),
    year: z.number(),
    active: z.boolean(),
    dnf: z.boolean(),
    price: z.number(),
    tooExpensive: z.boolean(),
    totalPoints: z.number(),
    totalParticipants: z.number(),
    selectedCount: z.number(),
    gcPosition: z.number().nullable(),
    pointsPosition: z.number().nullable(),
    komPosition: z.number().nullable(),
    youthPosition: z.number().nullable(),
});
export type RiderRaceOverviewRow = z.infer<typeof riderRaceOverviewRowSchema>;

export const riderSelectionHistoryRowSchema = z.object({
    username: z.string(),
    selectedCount: z.number(),
    totalCount: z.number(),
    races: z.array(z.string()),
});
export type RiderSelectionHistoryRow = z.infer<typeof riderSelectionHistoryRowSchema>;

export const riderOverviewSchema = z.object({
    riderId: z.number(),
    firstname: z.string(),
    lastname: z.string(),
    races: z.array(riderRaceOverviewRowSchema),
    selectionHistory: z.array(riderSelectionHistoryRowSchema),
});
export type RiderOverview = z.infer<typeof riderOverviewSchema>;
