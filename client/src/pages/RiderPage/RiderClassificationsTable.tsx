import Table from "@/components/ui/table/Table";
import type { RiderClassificationRow } from "./models/RiderRaceDetail";
import { formatPosition } from "@/lib/format";

interface RiderClassificationsTableProps {
    classifications: RiderClassificationRow[];
    active: boolean;
    onStageClick: (stagenr: number) => void;
}

const RiderClassificationsTable = ({ classifications, active, onStageClick }: RiderClassificationsTableProps) => (
    <div className="panel">
        <Table
            data={classifications}
            title="Uitslagen per klassement"
            rowKey="label"
            pointerOnHover
            fitContent
            onRowClick={(r) => {
                if (r.stagenr !== null) {
                    onStageClick(r.stagenr);
                }
            }}
        >
            {(col) => [
                col.text((r: RiderClassificationRow) => r.label, { name: "Etappe", width: "70px" }),
                col.text((r: RiderClassificationRow) => r.stagePos ?? "", { name: "Positie", width: "60px" }),
                col.text((r: RiderClassificationRow) => formatPosition(r.gc.position, active && r.label === "Eindstand"), {
                    name: "Algemeen",
                    width: "70px",
                }),
                col.text(
                    (r: RiderClassificationRow) => formatPosition(r.points.position, active && r.label === "Eindstand"),
                    { name: "Punten", width: "60px" }
                ),
                col.text((r: RiderClassificationRow) => formatPosition(r.kom.position, active && r.label === "Eindstand"), {
                    name: "Berg",
                    width: "60px",
                }),
                col.text(
                    (r: RiderClassificationRow) => formatPosition(r.youth.position, active && r.label === "Eindstand"),
                    { name: "Jongeren", width: "70px" }
                ),
            ]}
        </Table>
    </div>
);

export default RiderClassificationsTable;
