import { TableColumn } from "react-data-table-component";
import RiderLink from "../../../components/shared/RiderLink";
import { StageSelectableRider } from "../models/StageSelectableRider";
import SreDataTable from "../../../components/shared/SreDataTable";

export interface StageSelectionTeamProps {
    team: StageSelectableRider[];
    isFetching: boolean;
    addRider: (id: number) => void;
    removeRider: (id: number) => void;
    addKopman: (id: number) => void;
    removeKopman: (id: number) => void;
}

const conditionalRowStyles = [
    {
        when: (row: StageSelectableRider) => row.rider.dnf,
        style: {
            color: "#64748b",
        },
    },
];

const StageSelectionTeam = ({
    team,
    isFetching,
    addRider,
    removeRider,
    addKopman,
    removeKopman,
}: StageSelectionTeamProps) => {
    const columns: TableColumn<StageSelectableRider>[] = [
        {
            name: "Naam",
            width: "45%",
            cell: (row: StageSelectableRider) => (
                <RiderLink rider={row.rider.rider} />
            ),
        },
        {
            name: "Team",
            width: "35%",
            selector: (row: StageSelectableRider) => row.rider.team,
            sortable: true,
        },
        {
            name: "",
            width: "10%",
            cell: (row: StageSelectableRider) => {
                return row.selected ? (
                    <button
                        style={{ width: "26px", backgroundColor: "red" }}
                        onClick={() =>
                            removeRider(row.rider.riderParticipationId)
                        }>
                        -
                    </button>
                ) : team.filter((x) => x.selected).length < 9 &&
                    !row.rider.dnf ? (
                    <button
                        style={{ width: "26px", backgroundColor: "green" }}
                        onClick={() =>
                            addRider(row.rider.riderParticipationId)
                        }>
                        +
                    </button>
                ) : (
                    <></>
                );
            },
        },
        {
            name: "Kopman",
            width: "10%",
            cell: (row: StageSelectableRider) => {
                return row.isKopman ? (
                    <button
                        style={{ width: "26px", backgroundColor: "red" }}
                        onClick={() =>
                            removeKopman(row.rider.riderParticipationId)
                        }>
                        -
                    </button>
                ) : row.selected && !row.rider.dnf ? (
                    <button
                        style={{ width: "26px", backgroundColor: "green" }}
                        onClick={() =>
                            addKopman(row.rider.riderParticipationId)
                        }>
                        +
                    </button>
                ) : (
                    <></>
                );
            },
        },
    ];

    return <SreDataTable
        title={`Jouw opstelling ${team.filter((x) => x.selected).length}/9`}
        columns={columns}
        data={team}
        progressPending={isFetching}
        conditionalRowStyles={conditionalRowStyles}
        pointerOnHover
    />
};

export default StageSelectionTeam;
