import { useQuery } from "@tanstack/react-query";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { useMissedPoints } from "../Statistics/MissedPointsHook";
import { useUitvallers } from "../Statistics/UitvallersHook";
import axios from "axios";

export type RaceScoreData = {
    topScore: number,
    bottomScore: number, 
    average: number,
    usernamesAndScores: {
        username: string,
        score: number,
    }[],
    year: number,
    name: string,
    raceId: number,
}

export function useRaceWrap() {
    const budgetParticipation = useBudgetContext();

    const {data: missedPoints} = useMissedPoints();
    const {data: uitvallers} = useUitvallers();

    const {data: raceScoreData} = useQuery({
        queryKey: ["raceScoreData", budgetParticipation] as const,
        queryFn: ({queryKey}) => fetchRaceWrap(queryKey[1]),
        staleTime: 24 * 3600,
        gcTime: 24 * 3600,
    });
    
    return {missedPoints, uitvallers, raceScoreData}
}


async function fetchRaceWrap( budgetParticipation: boolean): Promise<RaceScoreData[]> {
    const res = await axios.get(`/api/RaceWrap`, { params: { budgetParticipation } });
    return res.data;
}