import DataTable, { TableColumn } from 'react-data-table-component';
import { MissedPointsData } from './MissedPoints';

const MissedPointsTable = ({ title, riders: missedPoints }: { title: string, riders: MissedPointsData[] }) => {
    const columns: TableColumn<MissedPointsData>[] = [
        {
            name: 'Etappe',
            width: '80px',
            cell: (row: MissedPointsData) => row.etappe,
        },
        {
            name: 'Behaald',
            width: '80px',
            cell: (row: MissedPointsData) => row.behaald
        },
        {
            name: 'Optimaal',
            width: '90px',
            cell: (row: MissedPointsData) => row.optimaal
        },
        {
            name: 'Gemist',
            width: '80px',
            cell: (row: MissedPointsData) => row.gemist
        },
    ];

    return (
        <div style={{ border: 'solid' }} >
            <DataTable
                title={title}
                columns={columns}
                data={missedPoints}
                striped
                dense
            />
        </div>
    );
}

export default MissedPointsTable;


