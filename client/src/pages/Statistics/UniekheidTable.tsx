import DataTable, { TableColumn } from "react-data-table-component";

export type UniekheidRow = {
    user: string;
    uniekheid: number;
};

const UniekheidTable = ({ data, title }: { data: UniekheidRow[], title: string }) => {
    const columns: TableColumn<UniekheidRow>[] = [
        {
            name: 'User',
            width: '100px',
            cell: (row: UniekheidRow) => row.user,
        },
        {
            name: 'Uniekheid',
            width: '130px',
            cell: (row: UniekheidRow) => row.uniekheid
        }
    ];

    return (
        <div style={{ margin: "5px" }} >
            <DataTable title={title} columns={columns} data={data} striped dense theme="dark" />
        </div>
    );
};

export default UniekheidTable;
