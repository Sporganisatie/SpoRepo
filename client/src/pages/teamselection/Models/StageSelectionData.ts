import { StageSelectableRider } from "../../Stage/models/StageSelectableRider"

export interface StageSelectionData {
    team: StageSelectableRider[],
    deadline: Date | null
}