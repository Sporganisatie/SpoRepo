import DataTable, { TableColumn } from 'react-data-table-component';

interface DataItem {
    stageNumber: string;
    usernamesAndScores: { username: string, score: number }[]
}

const EtappeUitslagenTable = ({ data }: any) => {
    const usernamesAndScoresLength = data[0]?.usernamesAndScores?.length ?? 0;
    console.log(usernamesAndScoresLength)
    const additionalColumns = Array.from({ length: usernamesAndScoresLength }, (_, i) => i);

    const columns: TableColumn<DataItem>[] = [
        {
            name: 'Etappe',
            selector: (row: DataItem) => row.stageNumber,
            width: '80px'
        },
        ...additionalColumns.map((column) => (
            {
                name: `${column + 1}e`,
                selector: (row: DataItem) => `${row.usernamesAndScores[column]?.username} (${row.usernamesAndScores[column]?.score})`,
                width: '120px'
            }))
    ];

    return (
        <DataTable
            title="Etappe Uitslagen"
            columns={columns}
            data={data}
            highlightOnHover
            striped
            dense
        />
    );
};

export default EtappeUitslagenTable;
