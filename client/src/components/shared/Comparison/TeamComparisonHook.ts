import axios from "axios";
import { useBudgetContext } from "../BudgetContextProvider";
import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { UserSelection } from "../../../models/UserSelection";
import { AllSelectedRiderRow } from "./AllSelectedRidersTable";

export function useTeamComparison() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useParams();
  if (!raceId) {
    throw new Error("Expected raceId to be defined");
  }

  const { data, isFetching } = useQuery({
    queryKey: ["teamComparison", raceId, budgetParticipation, stagenr],
    queryFn: () => fetchData(raceId, budgetParticipation, stagenr),
    initialData: { teams: [], counts: [] },
    staleTime: 10 * 60 * 1000,
  });

  function fetchData(
    raceId: string,
    budgetParticipation: boolean,
    stagenr?: string
  ): Promise<{ teams: UserSelection[]; counts: AllSelectedRiderRow[] }> {
    if (stagenr) {
      return axios
        .get(`/api/stageresult/comparison`, {
          params: { raceId, stagenr, budgetParticipation },
        })
        .then((res) => {
          return res.data;
        })
        .catch((error) => {
          throw error;
        });
    } else {
      return axios
        .get(`/api/race/comparison`, {
          params: { raceId, stagenr, budgetParticipation },
        })
        .then((res) => {
          return res.data;
        })
        .catch((error) => {
          throw error;
        });
    }
  }

  return {
    data,
    isFetching,
  };
}
