import { RiderParticipation } from "../../../models/RiderParticipation";

export interface StageSelectableRider {
    rider: RiderParticipation,
    selected: boolean,
    isKopman: boolean
}