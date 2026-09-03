import axios from "../../api/client";
import { useQuery } from "@tanstack/react-query";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { riderOverviewSchema, type RiderOverview } from "./models/RiderOverview";

export function useRiderPage(riderId?: string) {
  const budgetParticipation = useBudgetContext();

  const { data } = useQuery({
    queryKey: ["riderOverview", riderId, budgetParticipation] as const,
    queryFn: ({ queryKey }) => fetchRiderOverview(queryKey[1], queryKey[2]),
    enabled: riderId !== undefined,
    staleTime: 10_000,
  });

  return data;
}

async function fetchRiderOverview(riderId?: string, budgetParticipation?: boolean): Promise<RiderOverview> {
  if (riderId === undefined) {
    throw new Error("Expected riderId");
  }
  const res = await axios.get(`/api/rider`, { params: { riderId, budgetParticipation } });
  return riderOverviewSchema.parse(res.data);
}
