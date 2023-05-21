import DataTable, { TableColumn } from 'react-data-table-component';

interface EtappeUitslag {
    stageNumber: string;
    usernamesAndScores: { username: string, score: number }[]
}

const EtappeUitslagenTable = ({ data }: any) => {
    const length = data[0]?.usernamesAndScores?.length ?? 0;
    const additionalColumns = Array.from({ length }, (_, i) => i);

    const columns: TableColumn<EtappeUitslag>[] = [
        {
            name: 'Etappe',
            selector: (row: EtappeUitslag) => row.stageNumber,
            width: '80px'
        },
        ...additionalColumns.map((column) => (
            {
                name: `${column + 1}e`,
                selector: (row: EtappeUitslag) => `${row.usernamesAndScores[column]?.username} (${row.usernamesAndScores[column]?.score})`,
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
