import { Rider } from "./Rider";


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
}

export enum StageSelectedEnum {
  None,
  InTeam,
  InStageSelection
}
