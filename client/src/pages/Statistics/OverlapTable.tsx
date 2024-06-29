import DataTable, { TableColumn } from 'react-data-table-component';
import { OverlapRow } from './Overlap';

const OverlapTable = ({ overlaps, title }: { overlaps: OverlapRow[], title: string }) => {
    const columns: TableColumn<OverlapRow>[] = [
        {
            name: 'User',
            width: '100px',
            cell: (row: OverlapRow) => row.user,
        },
    ];

    if (overlaps[0] !== undefined) {
        for (let key in overlaps[0].overlaps) {
            columns.push(
                {
                    name: key,
                    width: '100px',
                    cell: (row: OverlapRow) => row.overlaps[key] === -1 ? "X" : row.overlaps[key],
                })
        }
    }

    return (
        <div style={{ margin: "5px" }} >
            <DataTable title={title} columns={columns} data={overlaps} striped dense theme="dark" />
        </div>
    );
}

export default OverlapTable;


