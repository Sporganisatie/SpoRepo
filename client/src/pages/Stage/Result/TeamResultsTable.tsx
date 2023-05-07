import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../../components/shared/RiderLink';
import { Rider } from '../../../models/Rider';

export interface Teamresults {
    rider: Rider
    stagescore: number,
    gcscore: number,
    pointsscore: number,
    komscore: number,
    yocscore: number,
    teamscore: number,
    totalscore: number,
}

const TeamResultsTable = ({ data }: {data: Teamresults[]}) => {
    const columns: TableColumn<Teamresults>[] = [
        {
            name: 'Renner',
            cell: (row: Teamresults) => <RiderLink rider={row.rider} />,
        },
        {
            name: 'Dag',
            cell: (row: Teamresults) => row.stagescore,
        },
        {
            name: 'AK',
            cell: (row: Teamresults) => row.gcscore,
        },
        {
            name: 'Punt',
            cell: (row: Teamresults) => row.pointsscore,
        },
        {
            name: 'Berg',
            cell: (row: Teamresults) => row.komscore,
        },
        {
            name: 'Jong',
            cell: (row: Teamresults) => row.yocscore,
        },
        {
            name: 'Team',
            cell: (row: Teamresults) => row.teamscore,
        },
        {
            name: "Totaal",
            cell: (row: Teamresults) => row.totalscore
        },
    ];

    return (
        <div style={{ width: "50%", borderStyle: "solid" }} >
            <DataTable
                columns={columns}
                data={data}
                striped
                dense
            />
        </div>
    );
}

export default TeamResultsTable;


