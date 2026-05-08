import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "../../../api/client";
import type { StageSelectionData } from "../models/StageSelectionData";
import { stageSelectionDataSchema } from "../models/StageSelectionData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";
import type { StageSelectableRider } from "../models/StageSelectableRider";

const RIDER_TYPE_ORDER = ["Klassement", "Klimmer", "Sprinter", "Tijdrijder", "Aanvaller", "Knecht"];

function sortTeamArray(team: StageSelectableRider[]): StageSelectableRider[] {
  return [...team].sort(
    (a, b) =>
      Number(a.rider.dnf) - Number(b.rider.dnf) ||
      Number(b.selected) - Number(a.selected) ||
      RIDER_TYPE_ORDER.indexOf(a.rider.type ?? "") - RIDER_TYPE_ORDER.indexOf(b.rider.type ?? "") ||
      b.rider.price - a.rider.price ||
      a.rider.rider.lastname.localeCompare(b.rider.rider.lastname)
  );
}

function compleetheid(team: StageSelectableRider[]): number {
  return (
    team.filter((rider) => rider.selected).length + (team.some((rider) => rider.isKopman) ? 1 : 0)
  );
}

function withUpdatedCount(
  data: StageSelectionData,
  team: StageSelectableRider[],
  budgetParticipation: boolean
): StageSelectionData {
  const count = compleetheid(team);
  const useBudgetSlot = budgetParticipation && data.budgetCompleet != null;
  return {
    ...data,
    team,
    compleet: useBudgetSlot ? data.compleet : count,
    budgetCompleet: useBudgetSlot ? count : data.budgetCompleet,
  };
}

function applyRiderToggle(
  data: StageSelectionData,
  riderParticipationId: number,
  selected: boolean,
  budgetParticipation: boolean
): StageSelectionData {
  const newTeam = sortTeamArray(
    data.team.map((entry) =>
      entry.rider.riderParticipationId === riderParticipationId
        ? { ...entry, selected, isKopman: selected ? entry.isKopman : false }
        : entry
    )
  );
  return withUpdatedCount(data, newTeam, budgetParticipation);
}

function applyKopmanToggle(
  data: StageSelectionData,
  riderParticipationId: number,
  selected: boolean,
  budgetParticipation: boolean
): StageSelectionData {
  const newTeam = data.team.map((entry) => ({
    ...entry,
    isKopman: entry.rider.riderParticipationId === riderParticipationId ? selected : false,
  }));
  return withUpdatedCount(data, newTeam, budgetParticipation);
}

export function useStageSelection() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();

  const queryKey = ["stageSelection", raceId, stagenr, budgetParticipation] as const;
  const otherModeKey = ["stageSelection", raceId, stagenr, !budgetParticipation] as const;
  const queryClient = useQueryClient();
  const { data, isLoading } = useQuery({
    queryKey,
    queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2], queryKey[3]),
    throwOnError: true,
  });

  async function fetchData(raceId: string, stagenr: string, budgetParticipation: boolean) {
    const { data } = await axios.get(`/api/StageSelection`, {
      params: { raceId, stagenr, budgetParticipation },
    });
    return stageSelectionDataSchema.parse(data);
  }

  function useToggleMutation(
    mutationFn: (id: number) => Promise<void>,
    update: (data: StageSelectionData, id: number) => StageSelectionData
  ) {
    return useMutation(
      {
        mutationFn,
        onMutate: async (id: number) => {
          await queryClient.cancelQueries({ queryKey });
          const previousSelection = queryClient.getQueryData<StageSelectionData>(queryKey);
          if (previousSelection) {
            queryClient.setQueryData<StageSelectionData>(queryKey, update(previousSelection, id));
          }
          return { previousSelection };
        },
        onError: (_err, _id, context) => {
          queryClient.setQueryData(queryKey, context?.previousSelection);
        },
        onSettled: () => {
          queryClient.invalidateQueries({ queryKey: otherModeKey, exact: true });
        },
      },
      queryClient
    );
  }

  async function addRider(id: number) {
    await axios.post(
      `/api/StageSelection/Rider?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${id}`
    );
  }
  async function removeRider(id: number) {
    await axios.delete(
      `/api/StageSelection/Rider?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${id}`
    );
  }
  async function addKopman(id: number) {
    await axios.post(
      `/api/StageSelection/Kopman?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${id}`
    );
  }
  async function removeKopman(id: number) {
    await axios.delete(
      `/api/StageSelection/Kopman?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${id}`
    );
  }

  const addRiderMutation = useToggleMutation(addRider, (data, id) =>
    applyRiderToggle(data, id, true, budgetParticipation)
  );
  const removeRiderMutation = useToggleMutation(removeRider, (data, id) =>
    applyRiderToggle(data, id, false, budgetParticipation)
  );
  const addKopmanMutation = useToggleMutation(addKopman, (data, id) =>
    applyKopmanToggle(data, id, true, budgetParticipation)
  );
  const removeKopmanMutation = useToggleMutation(removeKopman, (data, id) =>
    applyKopmanToggle(data, id, false, budgetParticipation)
  );

  return {
    data,
    isLoading,
    addRider: addRiderMutation.mutate,
    removeRider: removeRiderMutation.mutate,
    addKopman: addKopmanMutation.mutate,
    removeKopman: removeKopmanMutation.mutate,
  };
}
export type StageSelectionHook = ReturnType<typeof useStageSelection>;
