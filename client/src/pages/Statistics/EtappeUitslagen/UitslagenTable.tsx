import { TableColumn } from 'react-data-table-component';
import SreDataTable from '../../../components/shared/SreDataTable';

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
    const length = Math.max(...data.map((etappe: EtappeUitslag) => etappe.usernamesAndScores.length));
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
                selector: (row: EtappeUitslag) => row.usernamesAndScores[column] === undefined ? "" : `${row.usernamesAndScores[column]?.username} (${row.usernamesAndScores[column]?.score})`,
                width: '120px'
            }))
    ];

    return <SreDataTable
        title={(allRaces ? 'Race' : 'Etappe') + " Uitslagen"}
        columns={columns}
        data={data}
        conditionalRowStyles={conditionalRowStyles} />
}

export default UitslagenTable;
