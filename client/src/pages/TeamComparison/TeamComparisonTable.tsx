import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../components/shared/RiderLink';
import { StageSelectedEnum, StageComparisonRider, UserSelection } from '../../models/UserSelection';

const conditionalRowStyles = [
    {
        when: (row: StageComparisonRider) => row.selected == StageSelectedEnum.InStageSelection,
        style: {
            border: '2px white solid',
            backgroundColor: 'rgb(59, 59, 59)',
            color: 'lightgray'
        },
    },
    {
        when: (row: StageComparisonRider) => row.selected === StageSelectedEnum.InTeam,
        style: {
            border: '2px black dotted',
            backgroundColor: 'rgb(219, 219, 219)',
            color: 'black'
        },
    },
];

const TeamComparisonTable = (data: UserSelection) => {
    const columns: TableColumn<StageComparisonRider>[] = [
        {
            name: 'Positie',
            minWidth: '10px',
            cell: (row: StageComparisonRider) => row.stagePos == null || row.stagePos === 0 ? "" : row.stagePos + "e",
        },
        {
            name: 'Renner',
            minWidth: '200px',
            // width: "60%",
            cell: (row: StageComparisonRider) => row.rider == null ? "Totaal" : <RiderLink rider={row.rider} />,
        },
        {
            name: "Totaal",
            minWidth: '10px',
            // width: "20%",
            cell: (row: StageComparisonRider) => row.totalScore
        },
    ];

    return (
        <div style={{ border: 'solid' }} >
            <DataTable
                title={data.username}
                columns={columns}
                data={data.riders}
                conditionalRowStyles={conditionalRowStyles}
                striped
                dense
            />
        </div>
    );
}

export default TeamComparisonTable;


