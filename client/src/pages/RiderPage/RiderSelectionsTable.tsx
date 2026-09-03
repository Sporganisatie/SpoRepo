import Table from "@/components/ui/table/Table";
import type { RiderSelectionRow } from "./models/RiderRaceDetail";

interface RiderSelectionsTableProps {
    selections: RiderSelectionRow[];
    totalParticipants: number;
}

const RiderSelectionsTable = ({ selections, totalParticipants }: RiderSelectionsTableProps) => {
    const timesSelectedByAnyone = selections.length;
    const popularityPercentage = Math.round((timesSelectedByAnyone / totalParticipants) * 100);

    return (
        <div className="panel">
            <Table
                data={selections}
                title={`Geselecteerd (${timesSelectedByAnyone}/${totalParticipants} · ${popularityPercentage}%)`}
                rowKey="username"
                paginated
                fitContent
                rowClassName={(r) => (r.isLoggedInUser ? "current-user" : undefined)}
            >
                {(col) => [
                    col.text((r: RiderSelectionRow) => r.username, { name: "Naam", width: "130px" }),
                    col.text((r: RiderSelectionRow) => r.points, { name: "Punten", width: "70px" }),
                    col.text((r: RiderSelectionRow) => `${r.timesSelected}x`, { name: "Opgesteld", width: "80px" }),
                    col.text((r: RiderSelectionRow) => `${r.timesKopman}x`, { name: "Kopman", width: "70px" }),
                ]}
            </Table>
        </div>
    );
};

export default RiderSelectionsTable;
