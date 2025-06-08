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
        queryKey: ["missed_points", raceId, budgetParticipation] as const,
        queryFn: (): Promise<MissedPointsTableData[]> => axios.get(`/api/Statistics/missedPoints`, { params: { raceId, budgetParticipation } })
    });

    return {data, isFetching}
}