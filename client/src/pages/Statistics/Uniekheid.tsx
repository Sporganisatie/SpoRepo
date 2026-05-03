import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "../../api/client";
import type { UniekheidRow } from "./UniekheidTable";
import UniekheidTable from "./UniekheidTable";
import type { UniekheidRennerRow } from "./UniekheidRennersTable";
import UniekheidRennersTable from "./UniekheidRennersTable";

export type UniekheidData = {
  start: UniekheidRow[];
  huidig: UniekheidRow[];
  renners: UniekheidRennerRow[];
};

const Uniekheid = () => {
  document.title = "Uniekheid";

  const { raceId } = useParams();
  const budgetParticipation = useBudgetContext();
  const [data, setData] = useState<UniekheidData>({ start: [], huidig: [], renners: [] });

  useEffect(() => {
    axios
      .get(`/api/Statistics/Uniekheid`, {
        params: { raceId, budgetParticipation },
      })
      .then((res) => {
        setData(res.data);
      })
      .catch((error) => {
        throw error;
      });
  }, [raceId, budgetParticipation]);

  return (
    <div style={{ gap: "10px", display: "flex" }}>
      <UniekheidTable title="Aan de start" data={data.start} />
      <UniekheidTable title="Nu" data={data.huidig} />
      <div style={{ marginLeft: "200px" }}>
        <UniekheidRennersTable title="Uniekheid Renners" data={data.renners} />
      </div>
    </div>
  );
};

export default Uniekheid;
