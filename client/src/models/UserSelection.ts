import type { Rider } from "./Rider";

export interface UserSelection {
  username: string;
  riders: StageComparisonRider[];
  gemist: StageComparisonRider[];
}

export interface StageComparisonRider {
  rider: Rider;
  kopman: boolean;
  stagePos?: number;
  totalScore: number;
  selected: StageSelectedEnum;
  dnf: boolean;
}

export enum StageSelectedEnum {
  None,
  InTeam,
  InStageSelection,
}

export const stageSelectionRowClass = (
  selected: StageSelectedEnum | undefined,
): string | undefined => {
  if (selected === StageSelectedEnum.InStageSelection) return "selected";
  if (selected === StageSelectedEnum.InTeam) return "notselected";
  return undefined;
};
