import { TableColumn } from "react-data-table-component";
import SreDataTable from "../../components/shared/SreDataTable";

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

    return <SreDataTable title={title} columns={columns} data={data} />
};

export default UniekheidTable;
