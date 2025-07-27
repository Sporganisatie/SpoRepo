import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";

export type MissedPointsData = {
    etappe: string;
    behaald: number;
    optimaal: number;
    gemist: number;
};

export type MissedPointsTableData = {
    username: string;
    data: MissedPointsData[];
};

export function useMissedPoints() {
    const { raceId } = useParams();
    const budgetParticipation = useBudgetContext();

    const {data, isFetching} = useQuery({
        queryKey: ["missed_points", budgetParticipation, raceId] as const,
        queryFn: ({queryKey}) => fetchMissedPoints(queryKey[1], queryKey[2]),
        staleTime: 24 * 3600,
        gcTime: 24 * 3600,
    });

    return {data, isFetching}
}

async function fetchMissedPoints( budgetParticipation: boolean, raceId?: string): Promise<MissedPointsTableData[]> {
    const res = await axios.get(`/api/Statistics/missedPoints`, { params: { raceId, budgetParticipation } });
    return res.data;
}