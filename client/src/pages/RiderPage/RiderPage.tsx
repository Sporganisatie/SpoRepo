import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useRiderPage } from "./RiderPageHook";
import { useRiderRacePage } from "./RiderRacePageHook";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import RiderPageHeader from "./RiderPageHeader";
import RiderRaceNav from "./RiderRaceNav";
import RiderRaceOverviewTable from "./RiderRaceOverviewTable";
import RiderSelectionHistoryTable from "./RiderSelectionHistoryTable";
import RiderStageScoresTable from "./RiderStageScoresTable";
import RiderClassificationsTable from "./RiderClassificationsTable";
import RiderSelectionsTable from "./RiderSelectionsTable";
import "./riderPage.css";

const RiderPage = () => {
  const { riderId } = useParams();
  const navigate = useNavigate();
  const budgetParticipation = useBudgetContext();
  const rider = useRiderPage(riderId);
  const [selectedRaceId, setSelectedRaceId] = useState<number | undefined>(undefined);
  const [initialized, setInitialized] = useState(false);
  const raceDetail = useRiderRacePage(riderId, selectedRaceId?.toString());

  const fullName = rider ? `${rider.firstname} ${rider.lastname}` : "";
  document.title = fullName || "Renner";

  useEffect(() => {
    if (!rider || initialized) {
      return;
    }
    const activeRace = rider.races.find((r) => r.active);
    if (activeRace) {
      setSelectedRaceId(activeRace.raceId);
    }
    setInitialized(true);
  }, [rider, initialized]);

  if (!rider) {
    return <div className="rider-page v-stack" />;
  }

  const onStageClick = (stagenr: number) => navigate(`/${selectedRaceId}/stage/${stagenr}`);

  return (
    <div className="rider-page v-stack">
      <RiderPageHeader
        firstname={rider.firstname}
        lastname={rider.lastname}
        price={raceDetail?.price}
        tooExpensive={raceDetail?.tooExpensive}
        dnf={raceDetail?.dnf}
      />

      <RiderRaceNav races={rider.races} activeRaceId={selectedRaceId} onSelect={setSelectedRaceId} />

      {selectedRaceId === undefined && (
        <div className="rider-race-tables">
          <RiderRaceOverviewTable races={rider.races} onSelect={setSelectedRaceId} />
          <RiderSelectionHistoryTable history={rider.selectionHistory} />
        </div>
      )}

      {selectedRaceId !== undefined && raceDetail && (
        <div className="rider-race-tables">
          <RiderStageScoresTable
            stages={raceDetail.stages}
            budgetParticipation={budgetParticipation}
            onStageClick={onStageClick}
          />
          <RiderClassificationsTable
            classifications={raceDetail.classifications}
            active={raceDetail.active}
            onStageClick={onStageClick}
          />
          <RiderSelectionsTable
            selections={raceDetail.selections}
            totalParticipants={raceDetail.totalParticipants}
          />
        </div>
      )}
    </div>
  );
};

export default RiderPage;

