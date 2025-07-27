import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import { UitvallersData } from "./Uitvallers";

export function useUitvallers() {
    const { raceId } = useParams();
    const budgetParticipation = useBudgetContext();

    const {data, isFetching} = useQuery({
        queryKey: ["uitvallers", budgetParticipation, raceId] as const,
        queryFn: ({queryKey}) => fetchUitvallers(queryKey[1], queryKey[2]),
        staleTime: 24 * 3600,
        gcTime: 24 * 3600,
    });

    return {data, isFetching}
}

async function fetchUitvallers( budgetParticipation: boolean, raceId?: string): Promise<UitvallersData[]> {
    const res = await axios.get(`/api/Statistics/uitvallers`, { params: { raceId, budgetParticipation } });
    return res.data;
}