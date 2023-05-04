import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../components/shared/RiderLink';
import { SelectableEnum } from '../../models/SelectableEnum';
import { SelectableRider } from './Models/SelectableRider';
import SelectableRiderFoldout from './SelectableRiderFoldOut';

const conditionalRowStyles = [
    {
        when: (row: SelectableRider) => row.selectable !== SelectableEnum.Open,
        style: {
            backgroundColor: 'red',
            color: 'white',
        },
    },
    {
        when: (row: SelectableRider) => row.selectable === SelectableEnum.Selected,
        style: {
            backgroundColor: 'yellow',
        },
    },
];

const SelectableRidersTable = ({ data, loading, updateRider }: { data: SelectableRider[], loading: boolean, updateRider: (id: number, adding: boolean) => void }) => {
    const columns: TableColumn<SelectableRider>[] = [
        {
            name: 'Naam',
            width: "50",
            cell: (row: SelectableRider) => <RiderLink rider={row.details.rider} />
        },
        {
            name: 'Price',
            width: "100px",
            selector: (row: SelectableRider) => row.details.price
        },
        {
            name: 'Team',
            selector: (row: SelectableRider) => row.details.team
        },
        {
            cell: (row: SelectableRider) => {
                switch (row.selectable) {
                    case SelectableEnum.Open:
                        return <button style={{ width: "20px", backgroundColor: "green" }} onClick={() => updateRider(row.details.riderParticipationId, true)}>+</button>;
                    case SelectableEnum.Selected:
                        return <button style={{ width: "20px", backgroundColor: "red" }} onClick={() => updateRider(row.details.riderParticipationId, false)}>-</button>;
                    default:
                        return <></>;
                }
            }
        }
    ];

    return (

        <div style={{ width: "48%", borderStyle: "solid" }} >
            <DataTable
                title="Alle renners"
                columns={columns}
                data={data}
                progressPending={loading}
                conditionalRowStyles={conditionalRowStyles}
                expandableRows
                expandableRowsComponent={SelectableRiderFoldout}
                expandOnRowClicked
                expandableRowsHideExpander
                striped
                highlightOnHover
                pointerOnHover
                dense
            />
        </div>
    );
}

export default SelectableRidersTable;


