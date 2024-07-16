import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";

export enum StageStateEnum {
  None,
  Selection,
  Started,
}

export function useStage() {
  let { raceId, stagenr } = useParams();
  if (raceId === undefined || stagenr === undefined) {
    throw new Error("Expected race and stage numbers");
  }
  let navigate = useNavigate();

  function setStage(newStage: string) {
    stagenr = newStage;
    navigate(`/${raceId}/stage/${newStage}`);
  }

  const { data: stageState } = useQuery({
    queryKey: ["stage", raceId, stagenr],
    queryFn: () => fetchStage(raceId, stagenr),
    staleTime: 10000,
  });

  function fetchStage(
    raceId?: string,
    stagenr?: string
  ): Promise<StageStateEnum> {
    if (raceId === undefined || stagenr === undefined) {
      throw new Error("Expected race and stage numbers");
    }
    return axios
      .get(`/api/stage`, { params: { raceId, stagenr } })
      .then((res) => {
        return res.data;
      })
      .catch(function (error) {
        throw error;
      });
  }

  return {
    raceId,
    stagenr,
    stageState,
    setStage,
  };
}
