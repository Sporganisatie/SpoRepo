import { Rider } from "../../../models/Rider";
import { StageSelectedEnum } from "../../../models/UserSelection";
import { StageSelectableRider } from "./StageSelectableRider"

export interface StageSelectionData {
    team: StageSelectableRider[],
    deadline: Date | null,
    classifications: Classifications,
    compleet: number,
    budgetCompleet: number | null
}

export interface Classifications {
    gc: ClassificationRow[];
    points: ClassificationRow[];
    kom: ClassificationRow[];
    youth: ClassificationRow[];
    stage?: ClassificationRow[];
}

export interface ClassificationRow {
    rider: Rider;
    team: string;
    result: BaseResult;
    selected: StageSelectedEnum;
}

export interface BaseResult {
    position: number;
    result: string;
    selected: StageSelectedEnum;
    change: string;
}