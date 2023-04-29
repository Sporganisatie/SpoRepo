import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../components/shared/RiderLink';
import { RiderParticipation } from '../../models/RiderParticipation';


const TeamSelectionTable = ({ data, loading, removeRider }: { data: RiderParticipation[], loading: boolean, removeRider: (id: number) => void }) => {
    const columns: TableColumn<RiderParticipation>[] = [
        {
            name: 'Naam',
            cell: (row: RiderParticipation) => <RiderLink rider={row.rider} />
        },
        {
            name: 'Price',
            selector: (row: RiderParticipation) => row.price,
        },
        {
            name: 'Team',
            selector: (row: RiderParticipation) => row.team,
        },
        {
            cell: (row: RiderParticipation) => <button style={{ width: "20px", backgroundColor: "red" }} onClick={() => removeRider(row.riderParticipationId)}>-</button>
        }
    ];

    return (

        <div style={{ width: "50%", borderStyle: "solid" }} >

            <DataTable
                title={`Jouw team ${data.length}/20`}
                columns={columns}
                data={data}
                progressPending={loading}
                striped
                highlightOnHover
                pointerOnHover
                dense
            />
        </div>
    );
}

export default TeamSelectionTable;


