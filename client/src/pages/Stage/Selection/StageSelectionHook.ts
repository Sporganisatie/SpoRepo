import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import {
  StageSelectionData,
  stageSelectionDataSchema,
} from "../models/StageSelectionData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";
import { StageSelectableRider } from "../models/StageSelectableRider";

export function useStageSelection() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();

  const queryKey = [
    "stageSelection",
    raceId,
    stagenr,
    budgetParticipation,
  ] as const;
  const queryClient = useQueryClient();
  const { data, isLoading } = useQuery({
    queryKey,
    queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2], queryKey[3]),
    throwOnError: true,
    staleTime: 3_600_000,
    gcTime: 3_600_000,
  });

  async function fetchData(
    raceId: string,
    stagenr: string,
    budgetParticipation: boolean
  ) {
    try {
      const { data } = await axios.get(`/api/StageSelection`, {
        params: {
          raceId: raceId,
          stagenr: stagenr,
          budgetParticipation: budgetParticipation,
        },
      });
      return stageSelectionDataSchema.parse(data);
    } catch (error) {
      throw error;
    }
  }

  const addRiderMutation = useMutation(
    {
      mutationFn: addRider,
      onMutate: async (newRiderParticipationId) => {
        await queryClient.cancelQueries({ queryKey });
        const previousSelection = queryClient.getQueryData(queryKey);
        queryClient.setQueryData<StageSelectionData>(
          queryKey,
          (oldData?: StageSelectionData) => {
            if (!oldData) {
              return undefined;
            }
            handleMutation(
              oldData,
              newRiderParticipationId,
              true,
              budgetParticipation
            );
          }
        );
        return { previousSelection };
      },
      onError: (err, newRiderParticipationId, context) => {
        queryClient.setQueryData(queryKey, context?.previousSelection);
      },
      onSettled: () => {
        queryClient.invalidateQueries({ queryKey });
      },
    },
    queryClient
  );

  async function addRider(riderParticipationId: number) {
    await axios.post(
      `/api/StageSelection/Rider?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${riderParticipationId}`,
      {
        params: {
          riderParticipationId,
          raceId: raceId,
          budgetParticipation: budgetParticipation,
          stagenr: stagenr,
        },
      }
    );
    queryClient.invalidateQueries({
      queryKey: ["stageSelection", raceId, stagenr, budgetParticipation],
    });
  }

  const removeRiderMutation = useMutation(
    {
      mutationFn: removeRider,
      onMutate: async (newRiderParticipationId) => {
        await queryClient.cancelQueries({ queryKey });
        const previousSelection = queryClient.getQueryData(queryKey);
        queryClient.setQueryData<StageSelectionData>(
          queryKey,
          (oldData?: StageSelectionData) => {
            if (!oldData) {
              return undefined;
            }
            handleMutation(
              oldData,
              newRiderParticipationId,
              false,
              budgetParticipation
            );
          }
        );
        return { previousSelection };
      },
      onError: (err, newRiderParticipationId, context) => {
        queryClient.setQueryData(queryKey, context?.previousSelection);
      },
      onSettled: () => {
        queryClient.invalidateQueries({ queryKey });
      },
    },
    queryClient
  );

  async function removeRider(riderParticipationId: number) {
    await axios.delete(
      `/api/StageSelection/Rider?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${riderParticipationId}`,
      {
        params: {
          riderParticipationId,
          raceId: raceId,
          budgetParticipation: budgetParticipation,
          stagenr: stagenr,
        },
      }
    );
    queryClient.invalidateQueries({
      queryKey: ["stageSelection", raceId, stagenr, budgetParticipation],
    });
  }

  function handleMutation(
    oldData: StageSelectionData,
    riderParticipationId: number,
    selected: boolean,
    budgetParticipation: boolean
  ) {
    const riderIdx = oldData.team.findIndex(
      ({ rider }) => rider.riderParticipationId === riderParticipationId
    );
    if (riderIdx === -1) {
      throw new Error("Rider participation ID not found.");
    }
    oldData.team[riderIdx].selected = selected;
    if (!selected) {
      oldData.team[riderIdx].isKopman = false;
    }
    sortTeam(oldData);
    setCompleet(oldData, budgetParticipation);
  }

  const addKopmanMutation = useMutation(
    {
      mutationFn: addKopman,
      onMutate: async (newRiderParticipationId) => {
        await queryClient.cancelQueries({ queryKey });
        const previousSelection = queryClient.getQueryData(queryKey);
        queryClient.setQueryData<StageSelectionData>(
          queryKey,
          (oldData?: StageSelectionData) => {
            if (!oldData) {
              return undefined;
            }
            handleKopmanMutation(
              oldData,
              newRiderParticipationId,
              true,
              budgetParticipation
            );
          }
        );
        return { previousSelection };
      },
      onError: (err, newRiderParticipationId, context) => {
        queryClient.setQueryData(queryKey, context?.previousSelection);
      },
      onSettled: () => {
        queryClient.invalidateQueries({ queryKey });
      },
    },
    queryClient
  );

  async function addKopman(riderParticipationId: number) {
    await axios.post(
      `/api/StageSelection/Kopman?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${riderParticipationId}`,
      {
        params: {
          riderParticipationId,
          raceId: raceId,
          budgetParticipation: budgetParticipation,
          stagenr: stagenr,
        },
      }
    );
    queryClient.invalidateQueries({
      queryKey: ["stageSelection", raceId, stagenr, budgetParticipation],
    });
  }

  const removeKopmanMutation = useMutation(
    {
      mutationFn: removeKopman,
      onMutate: async (newRiderParticipationId) => {
        await queryClient.cancelQueries({ queryKey });
        const previousSelection = queryClient.getQueryData(queryKey);
        queryClient.setQueryData<StageSelectionData>(
          queryKey,
          (oldData?: StageSelectionData) => {
            if (!oldData) {
              return undefined;
            }
            handleKopmanMutation(
              oldData,
              newRiderParticipationId,
              false,
              budgetParticipation
            );
          }
        );
        return { previousSelection };
      },
      onError: (err, newRiderParticipationId, context) => {
        queryClient.setQueryData(queryKey, context?.previousSelection);
      },
      onSettled: () => {
        queryClient.invalidateQueries({ queryKey });
      },
    },
    queryClient
  );

  async function removeKopman(riderParticipationId: number) {
    await axios.delete(
      `/api/StageSelection/Kopman?raceId=${raceId}&stagenr=${stagenr}&budgetParticipation=${budgetParticipation}&riderParticipationId=${riderParticipationId}`,
      {
        params: {
          riderParticipationId,
          raceId: raceId,
          budgetParticipation: budgetParticipation,
          stagenr: stagenr,
        },
      }
    );
    queryClient.invalidateQueries({
      queryKey: ["stageSelection", raceId, stagenr, budgetParticipation],
    });
  }

  function handleKopmanMutation(
    oldData: StageSelectionData,
    riderParticipationId: number,
    selected: boolean,
    budgetParticipation: boolean
  ) {
    const riderIdx = oldData.team.findIndex(
      ({ rider }) => rider.riderParticipationId === riderParticipationId
    );
    if (riderIdx === -1) {
      throw new Error("Rider participation ID not found.");
    }
    oldData.team.forEach((rider) => (rider.isKopman = !selected));
    oldData.team[riderIdx].isKopman = selected;
    setCompleet(oldData, budgetParticipation);
  }

  function setCompleet(oldData: StageSelectionData, budgetParticipation: boolean) {
    if (budgetParticipation && oldData.budgetCompleet) {
      oldData.budgetCompleet = CompleetHeid(oldData.team);
    } else {
      oldData.compleet = CompleetHeid(oldData.team);
    }
  }

  function CompleetHeid(team: StageSelectableRider[]): number {
    return team.filter(rider => rider.selected).length + (team.some((rider) => rider.isKopman) ? 1 : 0);
  }

  const riderTypeOrder = [
    "Klassement",
    "Klimmer",
    "Sprinter",
    "Tijdrijder",
    "Aanvaller",
    "Knecht"
  ];

  function sortTeam(data: StageSelectionData) {
    data.team
      .sort(
        (a, b) => Number(a.rider.dnf) - Number(b.rider.dnf)
          || Number(b.selected) - Number(a.selected)
          || riderTypeOrder.indexOf(a.rider.type) - riderTypeOrder.indexOf(b.rider.type)
          || b.rider.price - a.rider.price
          || a.rider.rider.lastname.localeCompare(b.rider.rider.lastname)
      )
  }

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
