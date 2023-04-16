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
        when: (row: SelectableRider) => row.selectable == SelectableEnum.Selected,
        style: {
            backgroundColor: 'yellow',
        },
    },
];

const SelectableRidersTable = ({ data, loading, removeRider, addRider }: { data: SelectableRider[], loading: boolean, removeRider: (id: number) => void, addRider: (id: number) => void }) => {
    const columns: TableColumn<SelectableRider>[] = [
        {
            name: 'Naam',
            width: "50",
            cell: (row: SelectableRider) => <RiderLink rider={row.details.rider} />
        },
        {
            name: 'Price',
            width: "100px",
            selector: (row: SelectableRider) => row.details.price,
        },
        {
            name: 'Team',
            selector: (row: SelectableRider) => row.details.team,
        },
        {
            cell: (row: SelectableRider) => {
                switch (row.selectable) {
                    case SelectableEnum.Open:
                        return <button style={{ width: "20px", backgroundColor: "green" }} onClick={() => addRider(row.details.riderParticipationId)}>+</button>;
                    case SelectableEnum.Selected:
                        return <button style={{ width: "20px", backgroundColor: "red" }} onClick={() => removeRider(row.details.riderParticipationId)}>-</button>;
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


