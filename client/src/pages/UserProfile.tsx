import { useEffect, useMemo, useState } from "react";
import { useRaceContext } from "../components/shared/RaceContextProvider";
import type { TableColumn } from "react-data-table-component";
import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import axios from "../api/client";
import { useBudgetContext } from "../components/shared/BudgetContextProvider";
import SreDataTable from "../components/shared/SreDataTable";
import RiderLink from "../components/shared/RiderLink";
import type { Rider } from "../models/Rider";
import "./UserProfile.css";

type ProfilePage = "currentRace" | "overview";

type Race = {
  raceId: number;
  name: string;
  year: number;
};

type StageComparisonRider = {
  rider: Rider | null;
  totalScore: number;
  dnf: boolean;
};

type CombinedSelectedRider = {
  targetRider: StageComparisonRider;
  viewerRider: StageComparisonRider;
};

type HuidigeRaceTeams = {
  bothSelected: CombinedSelectedRider[];
  uniquePaired: CombinedSelectedRider[];
};

type UserProfileData = {
  username: string;
  viewerUsername: string;
  huidigeRaceTeams: HuidigeRaceTeams | null;
  overview: Race[];
};

const UserProfile = () => {
  const { username } = useParams();
  const budgetParticipation = useBudgetContext();

  const [selectedPage, setSelectedPage] = useState<ProfilePage | null>(null);
  const raceId = useRaceContext();

  const { data, isLoading } = useQuery({
    queryKey: ["userProfile", username, budgetParticipation, raceId] as const,
    queryFn: async ({ queryKey }) => {
      const response = await axios.get<UserProfileData>("/api/userprofile", {
        params: {
          username: queryKey[1],
          budgetParticipation: queryKey[2],
          raceId: queryKey[3] ?? undefined,
        },
      });
      return response.data;
    },
    throwOnError: true,
  });

  const canShowCurrentRace = Boolean(data?.huidigeRaceTeams);
  const page: ProfilePage =
    selectedPage === "currentRace" && !canShowCurrentRace
      ? "overview"
      : (selectedPage ?? (canShowCurrentRace ? "currentRace" : "overview"));

  useEffect(() => {
    document.title = data?.username ? `Profiel: ${data.username}` : "Profiel";
  }, [data?.username]);

  const overviewColumns = useMemo<TableColumn<Race>[]>(
    () => [
      {
        name: "Race",
        selector: (row) => `${row.name} ${row.year}`,
        sortable: true,
      },
    ],
    []
  );

  const bothSelectedColumns = useMemo<TableColumn<CombinedSelectedRider>[]>(
    () => [
      {
        name: "Rider",
        width: "200px",
        cell: (row) =>
          row.targetRider.rider === null ? (
            "Totaal"
          ) : (
            <RiderLink rider={row.targetRider.rider} />
          ),
      },
      {
        name: data?.username || "Target",
        width: "140px",
        center: true,
        cell: (row) => row.targetRider.totalScore,
      },
      {
        name: data?.viewerUsername,
        width: "140px",
        center: true,
        cell: (row) => row.viewerRider.totalScore,
      },
    ],
    [data?.username, data?.viewerUsername]
  );

  const uniquePairedColumns = useMemo<TableColumn<CombinedSelectedRider>[]>(
    () => [
      {
        name: data?.username || "Target",
        width: "180px",
        cell: (row) =>
          row.targetRider.rider === null ? (
            "Totaal"
          ) : (
            <RiderLink rider={row.targetRider.rider} />
          ),
      },
      {
        name: "Punten",
        width: "90px",
        center: true,
        cell: (row) => row.targetRider.totalScore,
      },
      {
        name: data?.viewerUsername,
        width: "180px",
        cell: (row) =>
          row.viewerRider.rider === null ? (
            "Totaal"
          ) : (
            <RiderLink rider={row.viewerRider.rider} />
          ),
      },
      {
        name: "Punten",
        width: "90px",
        center: true,
        cell: (row) => row.viewerRider.totalScore,
      },
    ],
    [data?.username, data?.viewerUsername]
  );




  if (isLoading || !data) {
    return <div className="user-profile-page" />;
  }

  return (
    <div className="user-profile-page">
      <div className="ts-panel">
        <div className="ts-panel-header user-profile-header">
          <h3 className="ts-panel-title">Profiel: {data.username}</h3>
        </div>
        <div className="user-profile-nav">
          {data.huidigeRaceTeams ? (
            <>
              <button
                className={`user-profile-tab ${page === "currentRace" ? "active" : ""}`}
                onClick={() => setSelectedPage("currentRace")}
              >
                Huidige race
              </button>
              <button
                className={`user-profile-tab ${page === "overview" ? "active" : ""}`}
                onClick={() => setSelectedPage("overview")}
              >
                Algemeen overzicht
              </button>
            </>
          ) : (
            <button
              className="user-profile-tab active"
              onClick={() => setSelectedPage("overview")}
            >
              Algemeen overzicht
            </button>
          )}
        </div>
      </div>

      {page === "currentRace" && data.huidigeRaceTeams && (
        <div className="ts-panel">
          <div className="ts-panel-header">
            <h3 className="ts-panel-title">Huidige race</h3>
          </div>
          <div style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
            <div style={{ maxWidth: "450px" }}>
              <SreDataTable
                columns={bothSelectedColumns}
                data={data.huidigeRaceTeams.bothSelected}
                conditionalRowStyles={[
                  {
                    when: (row) => row.targetRider.dnf || row.viewerRider.dnf,
                    style: {
                      textDecoration: "line-through",
                      color: "grey",
                    },
                  },
                ]}
              />
            </div>
            {data.huidigeRaceTeams.uniquePaired.length > 0 && (
              <div style={{ maxWidth: "550px" }}>
                <SreDataTable
                  columns={uniquePairedColumns}
                  data={data.huidigeRaceTeams.uniquePaired}
                  conditionalRowStyles={[
                    {
                      when: (row) => row.targetRider.dnf || row.viewerRider.dnf,
                      style: {
                        textDecoration: "line-through",
                        color: "grey",
                      },
                    },
                  ]}
                />
              </div>
            )}
          </div>
        </div>
      )}

      {page === "overview" && (
        <div className="ts-panel">
          <div className="ts-panel-header">
            <h3 className="ts-panel-title">Alle deelnames</h3>
          </div>
          <SreDataTable columns={overviewColumns} data={data.overview} />
        </div>
      )}
    </div>
  );
};

export default UserProfile;
