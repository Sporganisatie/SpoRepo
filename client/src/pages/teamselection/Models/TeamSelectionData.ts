import type { RiderParticipation } from "../../../models/RiderParticipation";
import type { SelectableRider } from "./SelectableRider";

export interface TeamSelectionData {
  budget: number;
  budgetOver: number;
  team: RiderParticipation[];
  allRiders: SelectableRider[];
  allTeams: string[];
}
