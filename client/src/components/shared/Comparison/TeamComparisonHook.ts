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
  var { data, isFetching } = useQuery({
    queryKey: ["teamComparison", raceId, budgetParticipation, stagenr] as const,
    queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2], queryKey[3]),
    staleTime: 3_600_000,
    gcTime: 3_600_000,
    throwOnError: true,
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

  function handleToggle(index: number): void {
    console.log("toggle index " + index)
    if (data) data.teams[index].hideUser = !data?.teams[index].hideUser;
    console.log(data)
    data = data;
  }

  return {
    data,
    isFetching,
    handleToggle
  };
}
