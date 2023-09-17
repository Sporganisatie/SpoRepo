import DataTable, { TableColumn } from 'react-data-table-component';
import { InputData } from './Klassementen';
import RiderLink from '../../components/shared/RiderLink';

const KlassementenTable = ({ title, riders, resultTitle }: { title: string, resultTitle: string, riders: InputData[] }) => {
    const columns: TableColumn<InputData>[] = [
        {
            name: 'Positie',
            width: '70px',
            cell: (row: InputData) => row.position,
        },
        {
            name: 'Naam',
            width: '180px',
            cell: (row: InputData) => <RiderLink rider={row.rider} />
        },
        {
            name: resultTitle,
            width: '120px',
            cell: (row: InputData) => row.result,
        },
        {
            name: 'Gekozen',
            width: '100px',
            cell: (row: InputData) => row.accounts.length
        },
        {
            name: 'Users',
            width: '300px',
            cell: (row: InputData) => row.accounts.join(', ')
        }
    ];

    return (
        <div style={{ border: 'solid' }} >
            <DataTable
                title={title}
                columns={columns}
                data={riders}
                striped
                dense
            />
        </div>
    );
}

export default KlassementenTable;


