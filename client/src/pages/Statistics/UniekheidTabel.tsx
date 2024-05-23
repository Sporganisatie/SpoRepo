import DataTable, { TableColumn } from 'react-data-table-component';
import { UniekheidData } from './TeamOverlapTables';

const UniekheidTable = ({ userUniekheden }: { userUniekheden: UniekheidData[] }) => {
    const columns: TableColumn<UniekheidData>[] = [
        {
            name: 'User',
            width: '100px',
            cell: (row: UniekheidData) => row.user,
        },
        {
            name: 'Uniekheid',
            width: '130px',
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


