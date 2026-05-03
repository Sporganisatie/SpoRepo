import type { RiderParticipation } from "../../../models/RiderParticipation";
import type { SelectableEnum } from "../../../models/SelectableEnum";

export interface SelectableRider {
  details: RiderParticipation;
  selectable: SelectableEnum;
}
