import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { stageResultDataSchema } from "../models/StageResultData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

export function useStageResult() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();
  const { data, isFetching } = useQuery({
    queryKey: ["stageResults", raceId, stagenr, budgetParticipation] as const,
    queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2], queryKey[3]),
    placeholderData: {
      userScores: [],
      teamResult: [],
      classifications: { gc: [], points: [], kom: [], youth: [] },
    },
    staleTime: 3_600_000,
    gcTime: 3_600_000,
  });

  async function fetchData(
    raceId: string,
    stagenr: string,
    budgetParticipation: boolean
  ) {
    const { data } = await axios.get(`/api/stageresult`, {
      params: { raceId, stagenr, budgetParticipation },
    });
    return stageResultDataSchema.parse(data);
  }

  return {
    data,
    isFetching,
  };
}
