import { Rider } from "./Rider";


export interface UserSelection {
  username: string;
  riders: StageComparisonRider[];
}

export interface StageComparisonRider {
  rider: Rider;
  stagePos?: number;
  totalScore: number;
  selected: StageSelectedEnum;
}

export enum StageSelectedEnum {
  None,
  InTeam,
  InStageSelection
}
