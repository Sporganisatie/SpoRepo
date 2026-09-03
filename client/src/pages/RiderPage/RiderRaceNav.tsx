import type { WheelEvent } from "react";
import type { RiderRaceOverviewRow } from "./models/RiderOverview";

interface RiderRaceNavProps {
    races: RiderRaceOverviewRow[];
    activeRaceId?: number;
    onSelect: (raceId: number | undefined) => void;
}

const onWheel = (e: WheelEvent<HTMLDivElement>) => {
    if (e.deltaY === 0) {
        return;
    }
    e.currentTarget.scrollLeft += e.deltaY;
    e.preventDefault();
};

const RiderRaceNav = ({ races, activeRaceId, onSelect }: RiderRaceNavProps) => (
    <div className="rider-race-nav" onWheel={onWheel}>
        <button
            type="button"
            className={`rider-race-nav-item rider-race-nav-back${activeRaceId === undefined ? " active" : ""}`}
            onClick={() => onSelect(undefined)}
        >
            Overzicht
        </button>
        {races.map((r) =>
            r.tooExpensive ? (
                <span key={r.raceId} className="rider-race-nav-item disabled">
                    {r.raceName} {r.year}
                </span>
            ) : (
                <button
                    key={r.raceId}
                    type="button"
                    className={`rider-race-nav-item${r.raceId === activeRaceId ? " active" : ""}`}
                    onClick={() => onSelect(r.raceId)}
                >
                    {r.raceName} {r.year}
                </button>
            )
        )}
    </div>
);

export default RiderRaceNav;

