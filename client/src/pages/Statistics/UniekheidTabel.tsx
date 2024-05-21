import DataTable, { TableColumn } from 'react-data-table-component';
import { UniekheidData } from './TeamOverlapTables';

const UniekheidTable = ({ userUniekheden }: { userUniekheden: UniekheidData[] }) => {
    const columns: TableColumn<UniekheidData>[] = [
        {
            name: 'User',
            width: '70px',
            cell: (row: UniekheidData) => row.user,
        },
        {
            name: 'Uniekheid',
            width: '300px',
            cell: (row: UniekheidData) => row.uniekheid
        }
    ];

    return (
        <div style={{ border: 'solid' }} >
            <DataTable
                title={"Uniekheid"}
                columns={columns}
                data={userUniekheden}
                striped
                dense
            />
        </div>
    );
}

export default UniekheidTable;


