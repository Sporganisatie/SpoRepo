import Table from "@/components/ui/table/Table";
import type { RiderRaceOverviewRow } from "./models/RiderOverview";
import { formatPrice, formatPosition } from "@/lib/format";

interface RiderRaceOverviewTableProps {
    races: RiderRaceOverviewRow[];
    onSelect: (raceId: number) => void;
}

const RiderRaceOverviewTable = ({ races, onSelect }: RiderRaceOverviewTableProps) => {
    const yearDividers = new Set<number>();
    let lastYear: number | undefined;
    for (const race of races) {
        if (race.year !== lastYear) {
            yearDividers.add(race.raceId);
            lastYear = race.year;
        }
    }

    return (
        <div className="panel rider-overview-table">
            <Table
                data={races}
                rowKey="raceId"
                pointerOnHover
                fitContent
                rowClassName={(r) =>
                    [r.tooExpensive ? "dim" : undefined, yearDividers.has(r.raceId) ? "row-year-divider" : undefined]
                        .filter(Boolean)
                        .join(" ") || undefined
                }
                onRowClick={(r) => {
                    if (!r.tooExpensive) {
                        onSelect(r.raceId);
                    }
                }}
            >
                {(col) => [
                    col.text((r: RiderRaceOverviewRow) => `${r.raceName} ${r.year}`, { name: "Race", width: "100px" }),
                    col.text((r: RiderRaceOverviewRow) => r.totalPoints, { name: "Punten", width: "45px" }),
                    col.text((r: RiderRaceOverviewRow) => formatPrice(r.price), {
                        name: "Prijs",
                        width: "45px",
                    }),
                    col.text((r: RiderRaceOverviewRow) => (r.dnf ? "DNF" : formatPosition(r.gcPosition, r.active)), {
                        name: "Algemeen",
                        width: "50px",
                    }),
                    col.text((r: RiderRaceOverviewRow) => (r.dnf ? "" : formatPosition(r.pointsPosition, r.active)), {
                        name: "Puntenklass.",
                        width: "55px",
                    }),
                    col.text((r: RiderRaceOverviewRow) => (r.dnf ? "" : formatPosition(r.komPosition, r.active)), {
                        name: "Berg",
                        width: "40px",
                    }),
                    col.text((r: RiderRaceOverviewRow) => (r.dnf ? "" : formatPosition(r.youthPosition, r.active)), {
                        name: "Jongeren",
                        width: "50px",
                    }),
                    col.text(
                        (r: RiderRaceOverviewRow) =>
                            `${r.selectedCount} / ${r.totalParticipants} (${Math.round(
                                (100 * r.selectedCount) / r.totalParticipants
                            )}%)`,
                        { name: "Geselecteerd", width: "90px" }
                    ),
                    col.text((r: RiderRaceOverviewRow) => (r.tooExpensive ? "Te duur" : ""), {
                        name: "",
                        width: "45px",
                    }),
                ]}
            </Table>
        </div>
    );
};

export default RiderRaceOverviewTable;
