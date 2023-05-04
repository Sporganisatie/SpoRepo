import { RiderParticipation } from "../../../models/RiderParticipation";
import { SelectableEnum } from "../../../models/SelectableEnum";

export interface StageSelectableRider {
    rider: RiderParticipation,
    selected: boolean,
    isKopman: boolean
}