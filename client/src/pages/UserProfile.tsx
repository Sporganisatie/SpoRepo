import { useMemo, useState } from "react";
import type { TableColumn } from "react-data-table-component";
import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import axios from "../api/client";
import { useBudgetContext } from "../components/shared/BudgetContextProvider";
import SreDataTable from "../components/shared/SreDataTable";
import "./UserProfile.css";

type ProfilePage = "currentRace" | "overview";

type Race = {
  raceId: number;
  name: string;
  year: number;
};

type UserProfileData = {
  username: string;
  currentRace: Race | null;
  overview: Race[];
};

const UserProfile = () => {
  const { username } = useParams();
  const budgetParticipation = useBudgetContext();
  const [selectedPage, setSelectedPage] = useState<ProfilePage | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ["userProfile", username ?? "self", budgetParticipation] as const,
    queryFn: async ({ queryKey }) => {
      const response = await axios.get<UserProfileData>("/api/userprofile", {
        params: {
          username: queryKey[1] === "self" ? undefined : queryKey[1],
          budgetParticipation: queryKey[2],
        },
      });
      return response.data;
    },
    throwOnError: true,
  });

  const canShowCurrentRace = Boolean(data?.currentRace);
  const hasCurrentRaceParticipation = Boolean(
    data?.currentRace && data.overview.some((race) => race.raceId === data.currentRace?.raceId)
  );
  const page: ProfilePage =
    selectedPage === "currentRace" && !canShowCurrentRace
      ? "overview"
      : (selectedPage ?? (canShowCurrentRace ? "currentRace" : "overview"));

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
          {data.currentRace && hasCurrentRaceParticipation ? (
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

      {page === "currentRace" && data.currentRace && hasCurrentRaceParticipation && (
        <div className="ts-panel">
          <div className="ts-panel-header">
            <h3 className="ts-panel-title">Huidige race</h3>
          </div>
          <div className="user-profile-text">
            {data.username} doet wel mee aan {data.currentRace.name}, {data.currentRace.year}
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
