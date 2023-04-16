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
            cell: (row: SelectableRider) => <RiderLink rider={row.details.rider} />
        },
        {
            name: 'Price',
            selector: (row: SelectableRider) => row.details.price,
        },
        {
            name: 'Team',
            selector: (row: SelectableRider) => row.details.team,
        },
        {
            cell: (row: SelectableRider) => row.selectable == SelectableEnum.Open ? <button onClick={() => addRider(row.details.riderParticipationId)}>+</button> : <></>
        },
        {
            cell: (row: SelectableRider) => row.selectable == SelectableEnum.Selected ? <button onClick={() => removeRider(row.details.riderParticipationId)}>-</button> : <></>
        }
    ];

    return (
        <DataTable
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
    );
}

export default SelectableRidersTable;


