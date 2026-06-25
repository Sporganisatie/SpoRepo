import { useMemo } from "react";
import { useQueryClient } from "@tanstack/react-query";
import type { StageSelectableRider } from "@/pages/Stage/models/StageSelectableRider";
import type { StageSelectionData } from "@/pages/Stage/models/StageSelectionData";
import Table from "@/components/ui/table/Table";
import SelectionsComplete, { type BarValue } from "./SelectionComplete";
import { useRaceName } from "@/components/shared/useRaceName";
import { useBudgetContext } from "@/components/shared/BudgetContextProvider";
import { useStage } from "@/pages/Stage/StageHook";

export interface StageSelectionTeamProps {
  team: StageSelectableRider[];
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

  return (
    <div className="panel">
      <div className="panel-header">
        <SelectionsComplete bars={bars} jerseyClass={jerseyClass} />
      </div>
      <Table
        data={stats.sorted}
        hideHeader
        pointerOnHover
        rowKey={(r) => r.rider.riderParticipationId}
        rowClassName={(r) => (r.rider.dnf ? "dim" : undefined)}
      >
        {(col) => [
          col.rider((r) => r.rider.rider, { width: "44%" }),
          col.text((r) => <span className="rider-team-text">{r.rider.team}</span>, {
            width: "36%",
            sortable: true,
            sortFn: (a, b) => a.rider.team.localeCompare(b.rider.team),
          }),
          col.text(
            (r) => (
              <ToggleCell
                active={r.selected}
                eligible={stats.selectedCount < 9 && !r.rider.dnf}
                onAdd={() => addRider(r.rider.riderParticipationId)}
                onRemove={() => removeRider(r.rider.riderParticipationId)}
                addGlyph="➤"
              />
            ),
            { width: "10%" },
          ),
          col.text(
            (r) => (
              <ToggleCell
                active={r.isKopman}
                eligible={r.selected && !r.rider.dnf}
                onAdd={() => addKopman(r.rider.riderParticipationId)}
                onRemove={() => removeKopman(r.rider.riderParticipationId)}
                addGlyph="★"
              />
            ),
            { width: "10%", align: "center" },
          ),
        ]}
      </Table>
    </div>
  );
};

export default StageSelectionTeam;
