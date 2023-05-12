import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../components/shared/RiderLink';
import { StageSelectedEnum, StageComparisonRider } from '../../models/UserSelection';

const conditionalRowStyles = [
    {
        when: (row: StageComparisonRider) => row.selected === StageSelectedEnum.InStageSelection,
        classNames: ["selected"]
    },
    {
        when: (row: StageComparisonRider) => row.selected === StageSelectedEnum.InTeam,
        classNames: ["notselected"]
    },
];

const TeamComparisonTable = ({ title, riders }: { title: string, riders: StageComparisonRider[] }) => {
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
            cell: (row: StageComparisonRider) => row.rider == null ? "Totaal" : <RiderLink rider={row.rider} kopman={row.kopman} />,
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
                title={title}
                columns={columns}
                data={riders}
                conditionalRowStyles={conditionalRowStyles}
                striped
                dense
            />
        </div>
    );
}

export default TeamComparisonTable;

