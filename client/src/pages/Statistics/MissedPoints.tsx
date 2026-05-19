import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "../../api/client";
import MissedPointsTable from "./MissedPointsTable";

export type MissedPointsData = {
  etappe: number;
  behaald: number;
  optimaal: number;
  gemist: number;
};

type MissedPointsTableData = {
  username: string;
  data: MissedPointsData[];
};

const MissedPoints = () => {
  document.title = "Gemiste punten";
  const { raceId } = useParams();
  const budgetParticipation = useBudgetContext();
  const [data, setData] = useState<MissedPointsTableData[]>([]);

  useEffect(() => {
    axios
      .get(`/api/Statistics/missedPoints`, { params: { raceId, budgetParticipation } })
      .then((res) => {
        setData(res.data);
      })
      .catch((error) => {
        document.title = "Zeg tegen Rens Error bij gemiste punten";
      });
  }, [raceId, budgetParticipation]);

  return (
    <div>
      <div className="cluster">
        {data.map((missedPoints, index) => (
          <MissedPointsTable
            key={index}
            title={missedPoints.username}
            riders={missedPoints.data}
          />
        ))}
      </div>
    </div>
  );
};

export default MissedPoints;
