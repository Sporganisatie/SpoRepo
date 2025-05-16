import { TableColumn } from "react-data-table-component";
import SreDataTable from "../../components/shared/SreDataTable";
import RiderLink from "../../components/shared/RiderLink";
import { RiderParticipation } from "../../models/RiderParticipation";

export type UniekheidRennerRow = {
    riderParticipation: RiderParticipation;
    uniekheid: number;
    accounts: string[];
};

const UniekheidRennersTable = ({ data, title }: { data: UniekheidRennerRow[], title: string }) => {
    const columns: TableColumn<UniekheidRennerRow>[] = [
        {
            name: 'Naam',
            width: '200px',
            cell: (row: UniekheidRennerRow) => <RiderLink rider={row.riderParticipation.rider} />,
            sortable: true
        },
        {
            name: 'Uniekheid',
            width: '120px',
            selector: (row: UniekheidRennerRow) => row.uniekheid,
            sortable: true
        },
        {
            name: 'Users',
            width: '300px',
            selector: (row: UniekheidRennerRow) => row.accounts.sort((a, b) => (a > b ? 1 : -1)).join(", "),
            sortable: true
        },
    ];

    return <SreDataTable title={title} columns={columns} data={data} />
};

export default UniekheidRennersTable;
