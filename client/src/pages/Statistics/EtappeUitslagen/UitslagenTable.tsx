import DataTable, { TableColumn } from 'react-data-table-component';

export interface EtappeUitslag {
    stageNumber: string;
    year: number;
    name: string;
    usernamesAndScores: { username: string, score: number }[]
}

const conditionalRowStyles = [
    {
        when: (row: EtappeUitslag) => row.name === 'Giro',
        style: {
            borderTop: 'solid',
        },
    }
];

const UitslagenTable = ({ data, allRaces }: { data: any, allRaces: boolean }) => {
    const length = data[0]?.usernamesAndScores?.length ?? 0;
    const additionalColumns = Array.from({ length }, (_, i) => i);

    const columns: TableColumn<EtappeUitslag>[] = [
        { // TODO conditional entire object?
            name: allRaces ? 'Race' : 'Etappe',
            selector: (row: EtappeUitslag) => allRaces ? `${row.name} ${row.year}` : row.stageNumber,
            width: allRaces ? '160px' : '80px'
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
            conditionalRowStyles={conditionalRowStyles}
            dense
        />
    );
};

export default UitslagenTable;
