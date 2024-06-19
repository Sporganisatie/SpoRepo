import DataTable, { TableColumn } from "react-data-table-component";
import { z } from "zod";

export const userScoreSchema = z.object({
    account: z.object({
        username: z.string(),
    }),
    stagescore: z.number(),
    totalscore: z.number(),
});

export type UserScore = z.infer<typeof userScoreSchema>;

const UserScoreTable = ({ data }: { data: UserScore[] }) => {
    const columns: TableColumn<UserScore>[] = [
        {
            name: "Naam",
            selector: (row: UserScore) => row.account.username,
            sortable: true,
        },
        {
            name: "Dag",
            selector: (row: UserScore) => row.stagescore,
            sortable: true,
        },
        {
            name: "Totaal",
            selector: (row: UserScore) => row.totalscore,
            sortable: true,
        },
    ];

    return (
        <div>
            <DataTable title="Poule stand" columns={columns} data={data} striped dense theme="dark" />
        </div>
    );
};

export default UserScoreTable;
