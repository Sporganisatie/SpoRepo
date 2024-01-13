import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { stageResultDataSchema } from "../models/StageResultData";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

export function useStageResult() {
  const budgetParticipation = useBudgetContext();
  const { raceId, stagenr } = useStage();
  const { data, isFetching } = useQuery({
    queryKey: ["stageResults", raceId, stagenr, budgetParticipation],
    queryFn: () => fetchData(raceId, stagenr, budgetParticipation),
    placeholderData: {
      userScores: [],
      teamResult: [],
      classifications: { gc: [], points: [], kom: [], youth: [] },
    },
    staleTime: 60 * 60 * 1000,
  });

  async function fetchData(
    raceId: string,
    stagenr: string,
    budgetParticipation: boolean
  ) {
    try {
      const { data } = await axios.get(`/api/stageresult`, {
        params: { raceId, stagenr, budgetParticipation },
      });
      return stageResultDataSchema.parse(data);
    } catch (err) {
      throw err;
    }
  }

  return {
    data,
    isFetching,
  };
}
