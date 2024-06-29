import { TableColumn } from "react-data-table-component";
import { Rider } from "../../../models/Rider";
import RiderLink from "../RiderLink";
import { StageSelectedEnum } from "../../../models/UserSelection";
import SreDataTable from "../SreDataTable";

export type AllSelectedRiderRow = {
    rider: Rider;
    count: number;
    users: string[];
    selected: StageSelectedEnum
};

const conditionalRowStyles = [
    {
        when: (row: AllSelectedRiderRow) => row.selected === StageSelectedEnum.InStageSelection,
        classNames: ["selected"]
    },
    {
        when: (row: AllSelectedRiderRow) => row.selected === StageSelectedEnum.InTeam,
        classNames: ["notselected"]
    }
];

const AllSelectedRiders = ({ riders }: { riders: AllSelectedRiderRow[] }) => {
    const columns: TableColumn<AllSelectedRiderRow>[] = [
        {
            name: 'Naam',
            width: '200px',
            cell: (row: AllSelectedRiderRow) => <RiderLink rider={row.rider} />,
            sortable: true
        },
        {
            name: '#',
            width: '60px',
            selector: (row: AllSelectedRiderRow) => row.count,
            sortable: true
        },
        {
            name: 'Users',
            width: '310px',
            selector: (row: AllSelectedRiderRow) => row.users.sort((a, b) => (a > b ? 1 : -1)).join(", "),
            sortable: true
        }
    ];

    return <SreDataTable title={"Alle Geselecteerd"} columns={columns} data={riders} conditionalRowStyles={conditionalRowStyles} />
}

export default AllSelectedRiders;