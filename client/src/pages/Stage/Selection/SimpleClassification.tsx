import DataTable, { TableColumn } from 'react-data-table-component';
import RiderLink from '../../../components/shared/RiderLink';
import { StageSelectedEnum } from '../../../models/UserSelection';
import { ClassificationRow } from '../models/StageSelectionData';

const classificationRowStyle = [
    {
        when: (row: ClassificationRow) => row.selected === StageSelectedEnum.InStageSelection,
        classNames: ["selected"]
    },
    {
        when: (row: ClassificationRow) => row.selected === StageSelectedEnum.InTeam,
        classNames: ["notselected"]
    },
];

const SimpleClassification = ({ rows, title, resultColName }: { rows: ClassificationRow[], title: string, resultColName: string }) => {
    const columns: TableColumn<ClassificationRow>[] = [
        {
            name: '',
            selector: (row: ClassificationRow) => row.position,
            sortable: true
        },
        {
            name: 'Naam',
            width: '200px',
            cell: (row: ClassificationRow) => <RiderLink rider={row.rider} />,
            sortable: true
        },
        {
            name: resultColName,
            selector: (row: ClassificationRow) => row.result
        }
    ];

    return (
        <div style={{ borderStyle: "solid" }} >
            <DataTable
                title={title}
                columns={columns}
                data={rows}
                conditionalRowStyles={classificationRowStyle}
                striped
                highlightOnHover
                pointerOnHover
                dense
            />
        </div>
    );
}

export default SimpleClassification;


