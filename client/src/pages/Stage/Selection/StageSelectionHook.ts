import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { stageSelectionDataSchema } from "../models/StageSelectionData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

export function useStageSelection() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();

  const queryClient = useQueryClient();
  const { data, isFetching } = useQuery({
    queryKey: ["stageSelection", raceId, stagenr, budgetParticipation],
    queryFn: () => fetchData(raceId, stagenr, budgetParticipation),
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

  return {
    data,
    isFetching,
    addRider,
    removeRider,
    addKopman,
    removeKopman,
  };
}
export type StageSelectionHook = ReturnType<typeof useStageSelection>;
