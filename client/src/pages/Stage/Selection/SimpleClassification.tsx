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

const SimpleClassification = ({ rows, title, resultColName, pagination }: { rows: ClassificationRow[], title: string, resultColName: string, pagination?: boolean }) => {
    const columns: TableColumn<ClassificationRow>[] = [
        {
            name: '',
            maxWidth: '100px',
            selector: (row: ClassificationRow) => row.position,
            sortable: true
        },
        {
            name: 'Naam',
            minWidth: '200px',
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
                pagination={pagination}
                paginationPerPage={20}
            />
        </div>
    );
}

export default SimpleClassification;


