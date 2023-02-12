import { TeamselectionRider } from "../../repositories/teamselectionRepository";

export function filterByPrice(
  minPrice: number,
  maxPrice: number,
  riders: TeamselectionRider[]
): TeamselectionRider[] {
  return riders.filter(({ price }) => price >= minPrice && price <= maxPrice);
}

export function filterByTeam(
  teamName: string,
  riders: TeamselectionRider[]
): TeamselectionRider[] {
  return riders.filter(({ team }) => team === teamName);
}

export function filterBySkill(
  key: keyof Pick<
    TeamselectionRider,
    "gc" | "punch" | "climb" | "sprint" | "tt"
  >,
  minValue: number,
  riders: TeamselectionRider[]
): TeamselectionRider[] {
  return riders.filter((rider) => rider[key] >= minValue);
}

export function sortByKey(
  key: keyof TeamselectionRider,
  riders: TeamselectionRider[]
): TeamselectionRider[] {
  return riders.sort((a, b) => (a[key] > b[key] ? 1 : -1));
}
