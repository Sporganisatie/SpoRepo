import Table from "@/components/ui/table/Table";
import type { RiderStageScoreRow } from "./models/RiderRaceDetail";

interface RiderStageScoresTableProps {
    stages: RiderStageScoreRow[];
    budgetParticipation: boolean;
    onStageClick: (stagenr: number) => void;
}

const RiderStageScoresTable = ({ stages, budgetParticipation, onStageClick }: RiderStageScoresTableProps) => (
    <div className="panel">
        <Table
            data={stages}
            title="Punten per etappe"
            rowKey="label"
            pointerOnHover
            fitContent
            rowClassName={(r) => (r.stagenr === null ? "row-total" : undefined)}
            onRowClick={(r) => {
                if (r.stagenr !== null) {
                    onStageClick(r.stagenr);
                }
            }}
        >
            {(col) => [
                col.text((r: RiderStageScoreRow) => r.label, { name: "Etappe", width: "70px" }),
                col.text((r: RiderStageScoreRow) => r.stageScore ?? "", { name: "Dag", width: "50px" }),
                col.text((r: RiderStageScoreRow) => r.gcScore, { name: "Algemeen", width: "70px" }),
                col.text((r: RiderStageScoreRow) => r.pointsScore, { name: "Punten", width: "60px" }),
                col.text((r: RiderStageScoreRow) => r.komScore, { name: "Berg", width: "60px" }),
                col.text((r: RiderStageScoreRow) => r.youthScore, { name: "Jongeren", width: "70px" }),
                col.text((r: RiderStageScoreRow) => r.teamScore, {
                    name: "Team",
                    width: "60px",
                    omit: budgetParticipation,
                }),
                col.text((r: RiderStageScoreRow) => r.totalScore, { name: "Totaal", width: "60px" }),
            ]}
        </Table>
    </div>
);

export default RiderStageScoresTable;
