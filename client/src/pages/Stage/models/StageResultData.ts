import { Teamresults } from "../Result/TeamResultsTable";
import { UserScore } from "../Result/UserScoreTable";

export interface StageResultData {
    userScores: UserScore[],
    teamResult: Teamresults[],
}