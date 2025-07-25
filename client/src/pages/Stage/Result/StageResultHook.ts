import { useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import {
  StageResultData,
  stageResultDataSchema,
} from "../models/StageResultData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

export function useStageResult() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();
  const queryClient = useQueryClient();
  const queryKey = [
    "stageResults",
    raceId,
    stagenr,
    budgetParticipation,
  ] as const;
  const { data, isFetching } = useQuery({
    queryKey,
    queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2], queryKey[3]),
    placeholderData: {
      userScores: [],
      teamResult: [],
      classifications: { gc: [], points: [], kom: [], youth: [] },
      virtualResult: false,
      finalStandings: false,
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
    const parsedData = stageResultDataSchema.parse(data);
    if (shouldAutoRefetch(parsedData)) {
      setTimeout(() => queryClient.invalidateQueries({ queryKey }), 30000);
    }
    return parsedData;
  }

  function shouldAutoRefetch(data: StageResultData) {
    if (!data.classifications.stage) {
      return false;
    }
    return (
      data.classifications.stage.length > 0 &&
      data.classifications.stage.length !== data.classifications.gc.length
    );
  }

  return {
    data,
    isFetching,
  };
}
