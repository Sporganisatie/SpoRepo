import { useMemo } from "react";
import type { TableColumn } from "react-data-table-component";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faShirt } from "@fortawesome/free-solid-svg-icons";
import { useQueryClient } from "@tanstack/react-query";
import RiderLink from "../../../components/shared/RiderLink";
import type { StageSelectableRider } from "../models/StageSelectableRider";
import type { StageSelectionData } from "../models/StageSelectionData";
import SreDataTable from "../../../components/shared/SreDataTable";
import SelectionsComplete, { type BarValue } from "./SelectionComplete";
import { useRaceName } from "../../../components/shared/useRaceName";
import { useBudgetContext } from "../../../components/shared/BudgetContextProvider";
import { useStage } from "../StageHook";

export interface StageSelectionTeamProps {
  team: StageSelectableRider[];
  isFetching: boolean;
  compleet: number;
  budgetCompleet: number | null;
  addRider: (id: number) => void;
  removeRider: (id: number) => void;
  addKopman: (id: number) => void;
  removeKopman: (id: number) => void;
}

function jerseyClassForRace(raceName: string | undefined): string {
  const n = raceName?.toLowerCase() ?? "";
  if (n.includes("giro")) return "giro";
  if (n.includes("tour")) return "tour";
  if (n.includes("vuelta")) return "vuelta";
  return "";
}

/** Decompose a `compleet` value (0-10) into selected riders + kopman flag.
 *  Used as a fallback when the other mode's team data isn't cached. Ambiguous
 *  below 10: assumes the user filled riders before picking a kopman. */
function decomposeCompleet(value: number): BarValue {
  if (value >= 10) return { selected: 9, kopman: true };
  return { selected: Math.max(0, value), kopman: false };
}

function deriveBar(team: StageSelectableRider[]): BarValue {
  return {
    selected: team.filter((x) => x.selected).length,
    kopman: team.some((x) => x.isKopman),
  };
}

const conditionalRowStyles = [
  {
    when: (row: StageSelectableRider) => row.rider.dnf,
    style: {
      color: "#64748b",
    },
  },
];

const StageSelectionTeam = ({
  team,
  isFetching,
  compleet,
  budgetCompleet,
  addRider,
  removeRider,
  addKopman,
  removeKopman,
}: StageSelectionTeamProps) => {
  const sortedTeam = useMemo(() => {
    const kopmanIdx = team.findIndex((r) => r.isKopman);
    if (kopmanIdx <= 0) return team;
    const next = [...team];
    const [kopman] = next.splice(kopmanIdx, 1);
    next.unshift(kopman);
    return next;
  }, [team]);
  const selectedCount = team.filter((x) => x.selected).length;
  const jerseyClass = jerseyClassForRace(useRaceName());
  const budgetParticipation = useBudgetContext();
  const queryClient = useQueryClient();
  const { raceId, stagenr } = useStage();

  const teamBar = deriveBar(team);

  // For the other mode, prefer the cached team[] (accurate); fall back to
  // decomposing the compleet number if that mode hasn't been loaded yet.
  const otherKey = ["stageSelection", raceId, stagenr, !budgetParticipation];
  const otherCached = queryClient.getQueryData<StageSelectionData>(otherKey);
  const otherCompleet = budgetParticipation ? compleet : budgetCompleet;
  const otherBar: BarValue | null = otherCached
    ? deriveBar(otherCached.team)
    : otherCompleet != null
      ? decomposeCompleet(otherCompleet)
      : null;

  const bars: BarValue[] =
    otherBar == null
      ? [teamBar]
      : budgetParticipation
        ? [otherBar, teamBar]
        : [teamBar, otherBar];

  const columns: TableColumn<StageSelectableRider>[] = [
    {
      name: "Naam",
      grow: 5,
      cell: (row: StageSelectableRider) => <RiderLink rider={row.rider.rider} />,
    },
    {
      name: "Team",
      grow: 4,
      cell: (row: StageSelectableRider) => (
        <span className="rider-team-text">{row.rider.team}</span>
      ),
      sortable: true,
      sortFunction: (a, b) => a.rider.team.localeCompare(b.rider.team),
    },
    {
      name: "",
      width: "10%",
      cell: (row: StageSelectableRider) => {
        if (row.selected) {
          return (
            <button
              className="teamselect-rider-button deselect"
              onClick={() => removeRider(row.rider.riderParticipationId)}
            >
              🞫
            </button>
          );
        }
        if (selectedCount < 9 && !row.rider.dnf) {
          return (
            <button
              className="teamselect-rider-button select"
              onClick={() => addRider(row.rider.riderParticipationId)}
            >
              ➤
            </button>
          );
        }
        return null;
      },
    },
    {
      name: (
        <FontAwesomeIcon
          icon={faShirt}
          className={`ss-completion-jersey active ${jerseyClass}`}
          title="Kopman"
        />
      ),
      width: "10%",
      center: true,
      cell: (row: StageSelectableRider) => {
        if (row.isKopman) {
          return (
            <button
              className="teamselect-rider-button deselect"
              onClick={() => removeKopman(row.rider.riderParticipationId)}
            >
              🞫
            </button>
          );
        }
        if (row.selected && !row.rider.dnf) {
          return (
            <button
              className="teamselect-rider-button select"
              onClick={() => addKopman(row.rider.riderParticipationId)}
            >
              ★
            </button>
          );
        }
        return null;
      },
    },
  ];

  return (
    <div className="ts-panel">
      <div className="ts-panel-header">
        <SelectionsComplete bars={bars} jerseyClass={jerseyClass} />
      </div>
      <SreDataTable
        columns={columns}
        data={sortedTeam}
        progressPending={isFetching}
        conditionalRowStyles={conditionalRowStyles}
        pointerOnHover
        noTableHead
      />
    </div>
  );
};

export default StageSelectionTeam;
