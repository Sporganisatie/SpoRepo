import { Rider } from "./Rider"

export interface RiderParticipation {
    riderParticipationId: number,
    raceId: number,
    riderId: number,
    price: number,
    dnf: boolean
    team: string,
    punch: number,
    climb: number,
    tt: number,
    sprint: number,
    gc: number,
    rider: Rider
}
