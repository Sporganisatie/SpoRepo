import { useState } from "react";
import TeamResultsTable from "./TeamResultsTable";
import UserScoreTable from "./UserScoreTable";
import StageClassifications from "./Classifications";
import TeamComparison from "../../../components/shared/Comparison/TeamComparison";
import { useStageResult } from "./StageResultHook";
import { useStage } from "../StageHook";
import Modal from "../../../components/ui/modal/Modal";

const StageResult = () => {
  const { stagenr } = useStage();
  document.title = `Etappe ${stagenr} resultaten`;
  const { data } = useStageResult();
  const [showTeamComparison, setShowTeamComparison] = useState<boolean>(false);

  return (
    <div>
      <div style={{ display: "grid" }}>
        {data?.virtualResult && (
          <div style={{ fontWeight: "bold", marginBottom: "10px", marginTop: "10px", fontSize: "2em" }}>
            Virtuele eindpunten o.b.v. de recentste etappe uitslag.
          </div>
        )}
        <button
          style={{ marginRight: "auto" }}
          className={showTeamComparison ? "active" : ""}
          onClick={() => setShowTeamComparison(!showTeamComparison)}
        >
          Alle Opstellingen
        </button>
        <Modal
          open={showTeamComparison}
          modalContents={<TeamComparison />}
          closeFn={() => setShowTeamComparison(false)}
          title="Alle opstellingen"
        />
      </div>
      {data ? (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gap: "10px",
            margin: "10px",
          }}
        >
          <div>
            <div style={{ marginBottom: "10px" }}>
              <TeamResultsTable data={data.teamResult} />
            </div>
            <UserScoreTable data={data.userScores} />
          </div>
          <div>
            <StageClassifications data={data.classifications} />
          </div>
        </div>
      ) : (
        <></>
      )}
    </div>
  );
};

export default StageResult;
