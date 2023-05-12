import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../../components/shared/RiderLink';
import { Rider } from '../../../models/Rider';

export interface RiderScore {
    rider: Rider
    kopman: boolean;
    stageScore: number,
    stagePos: number,
    classificationScore: number,
    teamScore: number,
    totalScore: number
}

const TeamResultsTable = ({ data }: { data: RiderScore[] }) => {
    const columns: TableColumn<RiderScore>[] = [
        {
            name: 'Positie',
            width: "12%",
            cell: (row: RiderScore) => row.stagePos == null || row.stagePos === 0 ? "" : row.stagePos + "e",
        },
        {
            name: 'Renner',
            width: "35%",
            cell: (row: RiderScore) => row.rider == null ? "Totaal" : <RiderLink rider={row.rider} kopman={row.kopman} />,
        },
        {
            name: 'Dag',
            width: "12%",
            cell: (row: RiderScore) => row.stageScore,
        },
        {
            name: 'Klassementen',
            width: "17%",
            cell: (row: RiderScore) => row.classificationScore,
        },
        {
            name: 'Team',
            width: "12%",
            cell: (row: RiderScore) => row.teamScore,
        },
        {
            name: "Totaal",
            width: "12%",
            cell: (row: RiderScore) => row.totalScore
        },
    ];

    return (
        <div style={{ borderStyle: "solid" }} >
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


