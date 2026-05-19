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

interface TeamStats {
  selectedCount: number;
  hasKopman: boolean;
  kopmanIdx: number;
  sorted: StageSelectableRider[];
}

function summarizeTeam(team: StageSelectableRider[]): TeamStats {
  let selectedCount = 0;
  let kopmanIdx = -1;
  for (let i = 0; i < team.length; i++) {
    if (team[i].selected) selectedCount++;
    if (team[i].isKopman && kopmanIdx === -1) kopmanIdx = i;
  }
  let sorted = team;
  if (kopmanIdx > 0) {
    sorted = [...team];
    const [kopman] = sorted.splice(kopmanIdx, 1);
    sorted.unshift(kopman);
  }
  return { selectedCount, hasKopman: kopmanIdx !== -1, kopmanIdx, sorted };
}

const conditionalRowStyles = [
  {
    when: (row: StageSelectableRider) => row.rider.dnf,
    style: { color: "#64748b" },
  },
];

interface ToggleCellProps {
  active: boolean;
  eligible: boolean;
  onAdd: () => void;
  onRemove: () => void;
  addGlyph: React.ReactNode;
}

const ToggleCell = ({ active, eligible, onAdd, onRemove, addGlyph }: ToggleCellProps) => {
  if (active) {
    return (
      <button className="teamselect-rider-button deselect" onClick={onRemove}>
        🞫
      </button>
    );
  }
  if (eligible) {
    return (
      <button className="teamselect-rider-button select" onClick={onAdd}>
        {addGlyph}
      </button>
    );
  }
  return null;
};

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
  const stats = useMemo(() => summarizeTeam(team), [team]);
  const jerseyClass = jerseyClassForRace(useRaceName());
  const budgetParticipation = useBudgetContext();
  const queryClient = useQueryClient();
  const { raceId, stagenr } = useStage();

  const teamBar: BarValue = { selected: stats.selectedCount, kopman: stats.hasKopman };

  // Other mode: prefer the cached team[] (accurate); fall back to decomposing
  // the compleet number if that mode hasn't been loaded yet.
  const otherKey = ["stageSelection", raceId, stagenr, !budgetParticipation];
  const otherCached = queryClient.getQueryData<StageSelectionData>(otherKey);
  const otherCompleet = budgetParticipation ? compleet : budgetCompleet;
  const otherBar: BarValue | null = otherCached
    ? {
        selected: otherCached.team.filter((x) => x.selected).length,
        kopman: otherCached.team.some((x) => x.isKopman),
      }
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
      cell: (row) => <RiderLink rider={row.rider.rider} />,
    },
    {
      name: "Team",
      grow: 4,
      cell: (row) => <span className="rider-team-text">{row.rider.team}</span>,
      sortable: true,
      sortFunction: (a, b) => a.rider.team.localeCompare(b.rider.team),
    },
    {
      name: "",
      width: "10%",
      cell: (row) => (
        <ToggleCell
          active={row.selected}
          eligible={stats.selectedCount < 9 && !row.rider.dnf}
          onAdd={() => addRider(row.rider.riderParticipationId)}
          onRemove={() => removeRider(row.rider.riderParticipationId)}
          addGlyph="➤"
        />
      ),
    },
    {
      name: (
        <FontAwesomeIcon
          icon={faShirt}
          className={`stage-select-completion-jersey active ${jerseyClass}`}
          title="Kopman"
        />
      ),
      width: "10%",
      center: true,
      cell: (row) => (
        <ToggleCell
          active={row.isKopman}
          eligible={row.selected && !row.rider.dnf}
          onAdd={() => addKopman(row.rider.riderParticipationId)}
          onRemove={() => removeKopman(row.rider.riderParticipationId)}
          addGlyph="★"
        />
      ),
    },
  ];

  return (
    <div className="panel">
      <div className="panel-header">
        <SelectionsComplete bars={bars} jerseyClass={jerseyClass} />
      </div>
      <SreDataTable
        columns={columns}
        data={stats.sorted}
        progressPending={isFetching}
        conditionalRowStyles={conditionalRowStyles}
        pointerOnHover
        noTableHead
      />
    </div>
  );
};

export default StageSelectionTeam;
