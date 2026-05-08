import { useQuery } from "@tanstack/react-query";
import axios from "../../api/client";
import { useRaceContext } from "./RaceContextProvider";

interface RaceOption {
  displayValue: string;
  value: number;
}

export function useRaceName(): string | undefined {
  const raceId = useRaceContext();
  const { data } = useQuery<RaceOption[]>({
    queryKey: ["race", "all"],
    queryFn: async () => (await axios.get("/api/race/all")).data,
    staleTime: 60_000,
  });
  return data?.find((r) => r.value === raceId)?.displayValue;
}
