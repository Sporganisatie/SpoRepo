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
  bothSelectedDnf: CombinedSelectedRider[];
  targetUnique: StageComparisonRider[];
  targetUniqueDnf: StageComparisonRider[];
  viewerUnique: StageComparisonRider[];
  viewerUniqueDnf: StageComparisonRider[];
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
        cell: (row) => {
          const targetRider = row.targetRider.rider;
          if (targetRider === null) {
            return "Totaal";
          }
          return <RiderLink rider={targetRider} />;
        },
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

  const uniqueRidersColumns = useMemo<TableColumn<StageComparisonRider>[]>(
    () => [
      {
        name: "Rider",
        width: "200px",
        cell: (row) => {
          if (row.rider === null) {
            return row.totalScore === -1 ? "" : "Totaal";
          }
          return <RiderLink rider={row.rider} />;
        },
      },
      {
        name: "Punten",
        width: "140px",
        center: true,
        cell: (row) => (row.totalScore === -1 ? "" : row.totalScore),
      },
    ],
    []
  );




  if (isLoading || !data) {
    return <div className="user-profile-page" />;
  }

  return (
    <div className="user-profile-page">
      <div className="user-profile-section">
        <div className="user-profile-header">
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
        <div className="user-profile-section">
          <div className="user-profile-columns user-profile-columns-current-race">
            <div className="user-profile-column user-profile-both-column">
              <div className="user-profile-column-title">Beide gekozen</div>
              <div className="user-profile-table-container">
                <SreDataTable
                  columns={bothSelectedColumns}
                  data={data.huidigeRaceTeams.bothSelected}
                />
              </div>
              {data.huidigeRaceTeams.bothSelectedDnf.length > 0 && (
                <div className="user-profile-dnf-section">
                  <h4 className="user-profile-dnf-title">DNF</h4>
                  <div className="user-profile-table-container">
                    <SreDataTable
                      columns={bothSelectedColumns}
                      data={data.huidigeRaceTeams.bothSelectedDnf}
                      conditionalRowStyles={[
                        {
                          when: () => true,
                          style: {
                            textDecoration: "line-through",
                            color: "var(--fg-muted)",
                          },
                        },
                      ]}
                    />
                  </div>
                </div>
              )}
            </div>

            <div className="user-profile-unique-grid">
              <div className="user-profile-column-title">{data.username}</div>
              <div className="user-profile-column-title user-profile-unique-viewer-title">
                {data.viewerUsername}
              </div>

              <div className="user-profile-table-container">
                <SreDataTable
                  columns={uniqueRidersColumns}
                  data={data.huidigeRaceTeams.targetUnique}
                />
              </div>
              <div className="user-profile-table-container user-profile-unique-viewer-column">
                <SreDataTable
                  columns={uniqueRidersColumns}
                  data={data.huidigeRaceTeams.viewerUnique}
                />
              </div>

              <div className="user-profile-dnf-section user-profile-unique-dnf-section">
                <h4 className="user-profile-dnf-title">DNF</h4>
                <div className="user-profile-table-container">
                  <SreDataTable
                    columns={uniqueRidersColumns}
                    data={data.huidigeRaceTeams.targetUniqueDnf}
                    conditionalRowStyles={[
                      {
                        when: () => true,
                        style: {
                          textDecoration: "line-through",
                          color: "var(--fg-muted)",
                        },
                      },
                    ]}
                  />
                </div>
              </div>
              <div className="user-profile-dnf-section user-profile-unique-dnf-section user-profile-unique-viewer-column">
                <h4 className="user-profile-dnf-title">DNF</h4>
                <div className="user-profile-table-container">
                  <SreDataTable
                    columns={uniqueRidersColumns}
                    data={data.huidigeRaceTeams.viewerUniqueDnf}
                    conditionalRowStyles={[
                      {
                        when: () => true,
                        style: {
                          textDecoration: "line-through",
                          color: "var(--fg-muted)",
                        },
                      },
                    ]}
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {page === "overview" && (
        <div className="user-profile-section">
          <h3 className="user-profile-section-title">Alle deelnames</h3>
          <SreDataTable columns={overviewColumns} data={data.overview} />
        </div>
      )}
    </div>
  );
};

export default UserProfile;
