import { StageSelectableRider } from "./StageSelectableRider"

export interface StageSelectionData {
    team: StageSelectableRider[],
    deadline: Date | null
}