import type { RiderParticipation } from "../../models/RiderParticipation";

const RIDER_TYPES: { key: string; label: string; cls: string }[] = [
  { key: "Klassement", label: "Klassement", cls: "klassement" },
  { key: "Klimmer", label: "Klimmer", cls: "klimmer" },
  { key: "Sprinter", label: "Sprinter", cls: "sprinter" },
  { key: "Tijdrijder", label: "Tijdrijder", cls: "tijdrijder" },
  { key: "Aanvaller", label: "Aanvaller", cls: "aanvaller" },
  { key: "Knecht", label: "Knecht", cls: "knecht" },
];

function formatM(value: number): string {
  return `${(value / 1_000_000).toFixed(1)}M`;
}

const RiderTypeTotals = ({ team }: { team: RiderParticipation[] }) => {
  return (
    <div className="rider-type-tiles">
      {RIDER_TYPES.map(({ key, label, cls }) => {
        const riders = team.filter((r) => r.type === key);
        const count = riders.length;
        const budget = riders.reduce((sum, r) => sum + r.price, 0);
        return (
          <div key={key} className={`rider-type-tile ${cls}`}>
            <div className="rider-type-tile-label">{label}</div>
            <div className="rider-type-tile-stats">
              <span className="rider-type-tile-count">{count}</span>
              <span className="rider-type-tile-budget">{formatM(budget)}</span>
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default RiderTypeTotals;
