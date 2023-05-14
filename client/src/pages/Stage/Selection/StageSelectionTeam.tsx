import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../../components/shared/RiderLink';
import { StageSelectableRider } from '../models/StageSelectableRider';

export interface StageSelectionTeamProps {
    data: StageSelectableRider[],
    loading: boolean,
    updateRider: (id: number, adding: boolean, type: string) => void,
}

const conditionalRowStyles = [
    {
        when: (row: StageSelectableRider) => row.rider.dnf,
        style: {
            backgroundColor: 'lightgrey',
            color: 'grey',
        },
    },
];

const StageSelectionTeam = ({ data, loading, updateRider }: StageSelectionTeamProps) => {
    const columns: TableColumn<StageSelectableRider>[] = [
        {
            name: 'Naam',
            width: '45%',
            cell: (row: StageSelectableRider) => <RiderLink rider={row.rider.rider} />,
            sortable: true
        },
        {
            name: 'Team',
            width: '30%',
            selector: (row: StageSelectableRider) => row.rider.team,
            sortable: true
        },
        {
            name: "",
            width: '5%',
            cell: (row: StageSelectableRider) => {
                return row.selected ? <button style={{ width: "20px", backgroundColor: "red" }} onClick={() => updateRider(row.rider.riderParticipationId, false, "rider")}>-</button>
                    : (data.filter(x => x.selected).length < 9 && !row.rider.dnf) ? <button style={{ width: "20px", backgroundColor: "green" }} onClick={() => updateRider(row.rider.riderParticipationId, true, "rider")}>+</button>
                        : <></>;
            }
        },
        {
            name: "Kopman",
            width: '20%',
            cell: (row: StageSelectableRider) => {
                return row.isKopman ? <button style={{ width: "20px", backgroundColor: "red" }} onClick={() => updateRider(row.rider.riderParticipationId, false, "kopman")}>-</button>
                    : (row.selected && !row.rider.dnf) ? <button style={{ width: "20px", backgroundColor: "green" }} onClick={() => updateRider(row.rider.riderParticipationId, true, "kopman")}>+</button>
                        : <></>;
            }
        }
    ];

    return (
        <div style={{ borderStyle: "solid" }} >
            <DataTable
                title={`Jouw opstelling ${data.filter(x => x.selected).length}/9`}
                columns={columns}
                data={data}
                progressPending={loading}
                conditionalRowStyles={conditionalRowStyles}
                striped
                highlightOnHover
                pointerOnHover
                dense
            />
        </div>
    );
}

export default StageSelectionTeam;


