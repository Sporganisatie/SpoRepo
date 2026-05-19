import type { TableColumn } from "react-data-table-component";
import RiderLink from "../../components/shared/RiderLink";
import type { RiderParticipation } from "../../models/RiderParticipation";
import { useEffect } from "react";
import SreDataTable from "../../components/shared/SreDataTable";
import BudgetMeter from "./BudgetMeter";

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
  removeRider,
  budget,
  budgetOver,
}: {
  data: RiderParticipation[];
  loading: boolean;
  removeRider: (id: number) => void;
  budget: number;
  budgetOver: number;
}) => {
  while (data.length < 20) {
    data.push({
      riderParticipationId: 0,
      raceId: 0,
      riderId: 0,
      price: 0,
      dnf: false,
      team: "",
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
        stickyTable.style.top = `${Math.max(0, relTop - Math.abs(top - 50))}px`;
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
      cell: (row: RiderParticipation) => {
        return (
          <button
            className="teamselect-rider-button deselect"
            onClick={() => removeRider(row.riderParticipationId)}
          >
            🞫
          </button>
        );
      },
    },
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
      selector: (row: RiderParticipation) => (row.price === 0 ? "" : row.price),
    },
    {
      name: "Team",
      selector: (row: RiderParticipation) => row.team,
    },
  ];

  const teamCount = data.filter((x) => x.riderParticipationId !== 0).length;
  const used = budget - budgetOver;

  return (
    <div
      id="stickyWrapper"
      style={{
        width: "100%",
        position: "relative",
      }}
    >
      <div
        id="stickyTable"
        className="panel rdt-action-col-first"
        style={{
          position: "absolute",
          top: "0px",
          width: "100%",
        }}
      >
        <div className="panel-header">
          <h3 className="panel-title">Mijn team {teamCount}/20</h3>
          <BudgetMeter used={used} total={budget} />
        </div>
        <div className="team-select-team-body">
          <SreDataTable
            columns={columns}
            data={data}
            conditionalRowStyles={conditionalRowStyles}
            progressPending={loading}
            pointerOnHover
          />
        </div>
      </div>
    </div>
  );
};

export default TeamSelectionTable;
