import DataTable, { TableColumn } from 'react-data-table-component';

interface UserRank {
    username: string;
    ranks: number[]
}

const RankingTable = ({ data }: any) => {
    const length = data.length ?? 0;
    const additionalColumns = Array.from({ length }, (_, i) => i);
    console.log(additionalColumns)
    const columns: TableColumn<UserRank>[] = [
        {
            name: 'User',
            selector: (row: UserRank) => row.username,
            width: '80px'
        },
        ...additionalColumns.map((column) => (
            {
                name: `${column + 1}e`,
                selector: (row: UserRank) => row.ranks[column],
                width: '50px'
            }))
    ];

    return (
        <DataTable
            title="Aantal keer per positie"
            columns={columns}
            data={data}
            highlightOnHover
            striped
            dense
        />
    );
};

export default RankingTable;
