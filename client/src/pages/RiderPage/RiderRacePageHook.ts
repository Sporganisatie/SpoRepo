import axios from "../../api/client";
import { useQuery } from "@tanstack/react-query";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { riderRaceDetailSchema, type RiderRaceDetail } from "./models/RiderRaceDetail";

export function useRiderRacePage(riderId?: string, raceId?: string) {
    const budgetParticipation = useBudgetContext();

    const { data } = useQuery({
        queryKey: ["riderRaceDetail", riderId, raceId, budgetParticipation] as const,
        queryFn: ({ queryKey }) => fetchRiderRaceDetail(queryKey[1], queryKey[2], queryKey[3]),
        enabled: riderId !== undefined && raceId !== undefined,
        staleTime: 10_000,
    });

    return data;
}

async function fetchRiderRaceDetail(
    riderId?: string,
    raceId?: string,
    budgetParticipation?: boolean
): Promise<RiderRaceDetail> {
    if (riderId === undefined || raceId === undefined) {
        throw new Error("Expected riderId and raceId");
    }
    const res = await axios.get(`/api/rider/race`, { params: { riderId, raceId, budgetParticipation } });
    return riderRaceDetailSchema.parse(res.data);
}
