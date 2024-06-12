import DataTable, { TableColumn } from "react-data-table-component";
import RiderLink from "../../components/shared/RiderLink";
import { RiderParticipation } from "../../models/RiderParticipation";
import { useEffect } from "react";

const conditionalRowStyles = [
    {
        when: (row: RiderParticipation) => row.riderId === 0,
        style: {
            color: "#64748b",
        },
    },
];

const TeamSelectionTable = ({
    data,
    loading,
}: {
    data: RiderParticipation[];
    loading: boolean;
}) => {
    while (data.length < 20) {
        data.push({
            riderParticipationId: 0,
            raceId: 0,
            riderId: 0,
            price: 0,
            dnf: false,
            team: "Team",
            punch: 0,
            climb: 0,
            tt: 0,
            sprint: 0,
            gc: 0,
            type: "",
            rider: {
                riderId: 0,
                firstname: "",
                lastname: "Lege plek",
                initials: "",
                country: "",
            },
        });
    }
    useEffect(() => {
        const handleScroll = () => {
            if (!stickyTable) {
                return;
            }
            const { top } = stickyTable.getBoundingClientRect();
            const relTop = Number(stickyTable.style.top.replace("px", ""));
            if (top < 50) {
                stickyTable.style.top = `${relTop + Math.abs(top - 50)}px`;
            }
            if (relTop > 0 && top > 50) {
                stickyTable.style.top = `${Math.max(
                    0,
                    relTop - Math.abs(top - 50)
                )}px`;
            }
        };

        const stickyTable = document.getElementById("stickyTable");

        window.addEventListener("scroll", handleScroll);

        return () => {
            window.removeEventListener("scroll", handleScroll);
        };
    }, []);

    const columns: TableColumn<RiderParticipation>[] = [
        {
            name: "Naam",
            cell: (row: RiderParticipation) => <RiderLink rider={row.rider} />,
        },
        {
            name: "Type",
            cell: (row: RiderParticipation) => row.type,
        },
        {
            name: "Price",
            selector: (row: RiderParticipation) => row.price,
        },
        {
            name: "Team",
            selector: (row: RiderParticipation) => row.team,
        },
    ];

    return (
        <div
            id="stickyWrapper"
            style={{
                width: "100%",
                position: "relative",
            }}>
            <div
                id="stickyTable"
                style={{
                    position: "absolute",
                    top: "0px",
                    width: "100%",
                    maxHeight: "calc(100vh - 50px)",
                    overflow: "auto",
                }}>
                <DataTable
                    columns={columns}
                    data={data}
                    conditionalRowStyles={conditionalRowStyles}
                    progressPending={loading}
                    striped
                    highlightOnHover
                    pointerOnHover
                    dense
                    theme="dark"
                />
            </div>
        </div>
    );
};

export default TeamSelectionTable;
