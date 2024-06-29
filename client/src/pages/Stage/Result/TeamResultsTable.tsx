import DataTable, { TableColumn } from "react-data-table-component";
import RiderLink from "../../../components/shared/RiderLink";
import { riderSchema } from "../../../models/Rider";
import { z } from "zod";

export const riderScoreSchema = z.object({
    rider: riderSchema.nullable(),
    kopman: z.boolean(),
    stageScore: z.number(),
    stagePos: z.number().nullable(),
    classificationScore: z.number(),
    teamScore: z.number(),
    totalScore: z.number(),
});

export type RiderScore = z.infer<typeof riderScoreSchema>;

const TeamResultsTable = ({ data }: { data: RiderScore[] }) => {
    const columns: TableColumn<RiderScore>[] = [
        {
            name: "Positie",
            width: "12%",
            cell: (row: RiderScore) =>
                row.stagePos == null || row.stagePos === 0
                    ? ""
                    : row.stagePos + "e",
        },
        {
            name: "Renner",
            width: "35%",
            cell: (row: RiderScore) =>
                row.rider == null ? (
                    "Totaal"
                ) : (
                    <RiderLink rider={row.rider} kopman={row.kopman} />
                ),
        },
        {
            name: "Dag",
            width: "12%",
            cell: (row: RiderScore) => row.stageScore,
        },
        {
            name: "Klassementen",
            width: "17%",
            cell: (row: RiderScore) => row.classificationScore,
        },
        {
            name: "Team",
            width: "12%",
            cell: (row: RiderScore) => row.teamScore,
        },
        {
            name: "Totaal",
            width: "12%",
            cell: (row: RiderScore) => row.totalScore,
        },
    ];

    return (
        <div>
            <DataTable
                columns={columns}
                data={data}
                striped
                dense
                theme="dark"
            />
        </div>
    );
};

export default TeamResultsTable;
