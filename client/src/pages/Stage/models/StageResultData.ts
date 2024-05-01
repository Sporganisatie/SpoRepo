import { RiderScore } from "../Result/TeamResultsTable";
import { UserScore } from "../Result/UserScoreTable";
import { Classifications } from "./StageSelectionData";

export interface StageResultData {
    userScores: UserScore[],
    teamResult: RiderScore[],
    classifications: Classifications
}