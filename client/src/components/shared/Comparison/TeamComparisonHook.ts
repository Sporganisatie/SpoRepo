import axios from "axios";
import { useBudgetContext } from "../BudgetContextProvider";
import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { UserSelection } from "../../../models/UserSelection";
import { AllSelectedRiderRow } from "./AllSelectedRidersTable";

export function useTeamComparison(setToggles: React.Dispatch<React.SetStateAction<{ username: string; showUser: boolean; }[]>>) {
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
    const apiUrl = stagenr ? `/api/stageresult/comparison` : `/api/race/comparison`;
    return axios
      .get(apiUrl, {
        params: { raceId, stagenr, budgetParticipation },
      })
      .then((res) => {
        const toggles = res.data.teams.map((team: UserSelection) => ({ username: team.username, showUser: true }));
        setToggles(toggles);
        return res.data;
      })
      .catch((error) => {
        throw error;
      });
  }

  function handleToggle(index: number): void {
    setToggles((prevToggles) => {
      const newToggles = [...prevToggles];
      newToggles[index].showUser = !newToggles[index].showUser;
      return newToggles;
    });
  }

  function toggleAll(): void {
    setToggles((prevToggles) => {
      const newValue = prevToggles.some(toggle => !toggle.showUser);
      return prevToggles.map(toggle => ({ ...toggle, showUser: newValue }));
    });
  }

  return {
    data,
    isFetching,
    handleToggle,
    toggleAll
  };
}
