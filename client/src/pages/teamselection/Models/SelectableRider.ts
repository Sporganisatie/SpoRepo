import { RiderParticipation } from "../../../models/RiderParticipation";
import { SelectableEnum } from "../../../models/SelectableEnum";

export interface SelectableRider {
    details: RiderParticipation,
    selectable: SelectableEnum
}