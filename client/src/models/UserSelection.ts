import { Rider } from "./Rider";


export interface UserSelection {
  username: string;
  riders: StageComparisonRider[];
  gemist: StageComparisonRider[];
  hideUser: boolean;
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
  InStageSelection
}
