import axios from "axios";
import { Rider } from "../models/Rider";

export interface TeamselectionRider {
  rider_participation_id: number;
  race_id: number;
  rider: Rider;
  price: number;
  dnf: boolean;
  team: string;
  punch: number;
  climb: number;
  tt: number;
  sprint: number;
  gc: number;
}

export default class TeamselectionRepository {
  getAll(raceId: number): Promise<TeamselectionRider[]> {
    return axios.get(`/api/teamselection/${raceId}/rider_participation`);
  }
}
