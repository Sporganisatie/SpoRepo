import type { RiderParticipation } from "../../models/RiderParticipation";
import { useEffect } from "react";
import Table from "../../components/ui/table/Table";
import BudgetMeter from "./BudgetMeter";

const TeamSelectionTable = ({
  data,
  removeRider,
  budget,
  budgetOver,
}: {
  data: RiderParticipation[];
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
        className="panel"
        style={{
          position: "absolute",
          top: "0px",
          width: "100%",
        }}
      >
        <div className="panel-header">
          <BudgetMeter used={used} total={budget} openSpaces={20 - teamCount} />
        </div>
        <div className="team-select-team-body">
          <Table
            data={data}
            pointerOnHover
            rowClassName={(r) => (r.riderId === 0 ? "dim" : undefined)}
          >
            {(col) => [
              col.text(
                (r) =>
                  r.riderParticipationId === 0 ? null : (
                    <button
                      className="teamselect-rider-button deselect"
                      onClick={() => removeRider(r.riderParticipationId)}
                    >
                      🞫
                    </button>
                  ),
                { width: "2.7rem", padding: "0" },
              ),
              col.rider((r) => r.rider, { name: "Naam" }),
              col.text((r) => r.type, { name: "Type" }),
              col.text((r) => (r.price === 0 ? "" : r.price), { name: "Price" }),
              col.text((r) => r.team, { name: "Team" }),
            ]}
          </Table>
        </div>
      </div>
    </div>
  );
};

export default TeamSelectionTable;
