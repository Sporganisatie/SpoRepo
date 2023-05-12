import { Rider } from "../../../models/Rider";
import { StageSelectedEnum } from "../../../models/UserSelection";
import { StageSelectableRider } from "./StageSelectableRider"

export interface StageSelectionData {
    team: StageSelectableRider[],
    deadline: Date | null,
    classifications: Classifications
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
    position: number;
    result: string;
    selected: StageSelectedEnum
}