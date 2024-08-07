import { TableColumn } from "react-data-table-component";
import RiderLink from "../../../components/shared/RiderLink";
import { StageSelectedEnum } from "../../../models/UserSelection";
import { ClassificationRow } from "../models/StageSelectionData";
import SreDataTable from "../../../components/shared/SreDataTable";

const classificationRowStyle = [
    {
        when: (row: ClassificationRow) =>
            row.selected === StageSelectedEnum.InStageSelection,
        classNames: ["selected"],
    },
    {
        when: (row: ClassificationRow) =>
            row.selected === StageSelectedEnum.InTeam,
        classNames: ["notselected"],
    },
];

const ClassificationTable = ({
    rows,
    title,
    resultColName,
    pagination,
    showRankChange,
}: {
    rows: ClassificationRow[];
    title: string;
    resultColName: string;
    pagination?: boolean;
    showRankChange?: boolean;
}) => {
    const columns: TableColumn<ClassificationRow>[] = [
        {
            name: "",
            width: "30px",
            style: {
                paddingLeft: 0,
                paddingRight: 0,
                flexGrow: 0,
                display: "flex",
                justifyContent: "center",
            },
            selector: (row: ClassificationRow) => row.result.position,
        },
        {
            name: "",
            width: "70px",
            selector: (row: ClassificationRow) => row.result.change ?? "",
            omit: !showRankChange,
            conditionalCellStyles: [
                {
                    when: (row) => row.result.change?.startsWith("▼") ?? false,
                    style: {
                        color: "red",
                    },
                },
                {
                    when: (row) => row.result.change?.startsWith("▲") ?? false,
                    style: {
                        color: "green",
                    },
                },
            ],
        },
        {
            name: "Naam",
            minWidth: "200px",
            cell: (row: ClassificationRow) => <RiderLink rider={row.rider} />,
        },
        {
            name: "Team",
            minWidth: "200px",
            cell: (row: ClassificationRow) => row.team,
        },
        {
            name: resultColName,
            selector: (row: ClassificationRow) => row.result.result,
        },
    ];

    return <SreDataTable
        title={title}
        columns={columns}
        data={rows}
        conditionalRowStyles={classificationRowStyle}
        pointerOnHover
        pagination={pagination}
        paginationPerPage={20} 
        noTableHead
    />
};

export default ClassificationTable;
