import DataTable, { TableColumn } from 'react-data-table-component';

export interface UserScore {
    account: {
        username: string,
    },
    stagescore: number,
    totalscore: number,
}

const UserScoreTable = ({ data }: { data: UserScore[] }) => {
    const columns: TableColumn<UserScore>[] = [
        {
            name: 'Naam',
            cell: (row: UserScore) => row.account.username,
            sortable: true
        },
        {
            name: 'Dag',
            cell: (row: UserScore) => row.stagescore,
            sortable: true
        },
        {
            name: "Totaal",
            cell: (row: UserScore) => row.totalscore,
            sortable: true
        },
    ];

    return (
        <div style={{ width: "50%", borderStyle: "solid" }} >
            <DataTable
                title="Poule stand"
                columns={columns}
                data={data}
                striped
                dense
            />
        </div>
    );
}

export default UserScoreTable;


