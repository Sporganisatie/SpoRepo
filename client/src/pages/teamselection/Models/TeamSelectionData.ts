import { RiderParticipation } from "../../../models/RiderParticipation";
import { SelectableRider } from "./SelectableRider";

export interface TeamSelectionData {
    budget: number,
    budgetOver: number,
    team: RiderParticipation[],
    allRiders: SelectableRider[]
}