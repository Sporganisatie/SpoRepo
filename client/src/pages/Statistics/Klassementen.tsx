import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "../../api/client";
import KlassementenTable from "./KlassementenTable";
import type { Rider } from "../../models/Rider";

export type InputData = {
  position: number;
  result: string;
  rider: Rider;
  price: number;
  accounts: string[];
};

const Klassementen = () => {
  document.title = "Klassementen";
  const { raceId } = useParams();
  const budgetParticipation = useBudgetContext();
  const [data, setData] = useState<InputData[][]>([]);

  useEffect(() => {
    axios
      .get(`/api/Statistics/klassementen`, { params: { raceId, budgetParticipation } })
      .then((res) => {
        setData(res.data);
      });
  }, [raceId, budgetParticipation]);

  return (
    <div>
      <div className="cluster">
        <KlassementenTable key="Algemeen" title="Algemeen" riders={data[0]} resultTitle="Tijd" />
        <KlassementenTable key="Punten" title="Punten" riders={data[1]} resultTitle="Punten" />
      </div>
      <div className="cluster">
        <KlassementenTable key="Berg" title="Berg" riders={data[2]} resultTitle="Punten" />
        <KlassementenTable key="Jongeren" title="Jongeren" riders={data[3]} resultTitle="Tijd" />
      </div>
    </div>
  );
};

export default Klassementen;
