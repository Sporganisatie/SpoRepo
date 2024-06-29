import DataTable, { TableColumn } from "react-data-table-component";
import {
    StageComparisonRider,
    StageSelectedEnum,
} from "../../../models/UserSelection";
import RiderLink from "../RiderLink";

const conditionalRowStyles = [
    {
        when: (row: StageComparisonRider) =>
            row.selected === StageSelectedEnum.InStageSelection,
        classNames: ["selected"],
    },
    {
        when: (row: StageComparisonRider) =>
            row.selected === StageSelectedEnum.InTeam,
        classNames: ["notselected"],
    },
    {
        when: (row: StageComparisonRider) => row.dnf,
        style: {
            textDecoration: "line-through",
            color: "grey",
        },
    },
];

const TeamComparisonTable = ({
    title,
    riders,
}: {
    title: string;
    riders: StageComparisonRider[];
}) => {
    const columns: TableColumn<StageComparisonRider>[] = [
        {
            name: "Positie",
            minWidth: "10px",
            cell: (row: StageComparisonRider) =>
                row.stagePos == null || row.stagePos === 0
                    ? ""
                    : row.stagePos + "e",
        },
        {
            name: "Renner",
            minWidth: "200px",
            // width: "60%",
            cell: (row: StageComparisonRider) =>
                row.rider == null ? (
                    row.totalScore === -1 ? (
                        ""
                    ) : (
                        "Totaal"
                    )
                ) : (
                    <RiderLink rider={row.rider} kopman={row.kopman} />
                ),
        },
        {
            name: "Totaal",
            minWidth: "10px",
            // width: "20%",
            cell: (row: StageComparisonRider) =>
                row.totalScore === -1 ? "" : row.totalScore,
        },
    ];

    return (
        <div>
            <DataTable
                title={title}
                columns={columns}
                data={riders}
                conditionalRowStyles={conditionalRowStyles}
                striped
                dense
                theme="dark"
            />
        </div>
    );
};

export default TeamComparisonTable;
