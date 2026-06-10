import { useState } from "react";
import axios from "../api/client";

type AdminError = {
  message?: string;
  type?: string;
  stackTrace?: string;
  inner?: string;
};

const Admin = () => {
  document.title = "Admin";
  const [stagenr, setStagenr] = useState("");
  const [year, setYear] = useState("");
  const [raceName, setRace] = useState("");

  const [raceId, setRaceId] = useState("");
  const [year2, setYear2] = useState("");
  const [raceName2, setRace2] = useState("");
  const [statsRaceId, setStatsRaceId] = useState("");
  const [statsStageNr, setStatsStageNr] = useState("");
  const [apStatsRaceId, setApStatsRaceId] = useState("");

  const [error, setError] = useState<AdminError | string | null>(null);

  const handleError = (err: any) => {
    console.error(err);
    setError(err?.response?.data ?? err?.message ?? "Unknown error");
  };

  const call = (url: string, params?: any) => {
    setError(null);
    axios
      .get(url, { params })
      .then((_) => { })
      .catch(handleError);
  };

  const updateResult = (params: any) => call(`/api/Admin/stageResults`, params);
  const updateStartlist = (params: any) => call(`/api/Admin/startlist`, params);
  const raceFinished = (params: any) => call(`/api/Admin/RaceFinished`, params);
  const etappesToevoegen = (params: any) => call(`/api/Admin/AddStages`, params);
  const downloadStageProfiles = (params: any) => call(`/api/Admin/DownloadStageProfiles`, params);
  const calculateStageSelectionStats = (params: any) => call(`/api/Admin/CalculateStageSelectionStats`, params);
  const calculateAccountParticipationStats = (params: any) => call(`/api/Admin/CalculateAccountParticipationStats`, params);
  const resetCache = () => call(`/api/Admin/resetCache`);

  const [diag, setDiag] = useState<any>(null);
  const playwrightDiag = () => {
    setError(null);
    setDiag(null);
    axios
      .get(`/api/Admin/playwrightDiag`)
      .then(r => setDiag(r.data))
      .catch(handleError);
  };

  return (
    <div>
      <div>
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={stagenr}
          onChange={(e) => setStagenr(e.target.value)}
          placeholder="stage"
        />
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={raceName}
          onChange={(e) => setRace(e.target.value)}
          placeholder="racename"
        />
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={year}
          onChange={(e) => setYear(e.target.value)}
          placeholder="year"
        />
        <button
          style={{ marginRight: "5px", width: "110px" }}
          onClick={() => updateResult({ raceName, year, stagenr })}
        >
          Uitslag scrape
        </button>
        <button
          style={{ marginRight: "5px", width: "180px" }}
          onClick={() => updateResult({ mostRecentStarted: true })}
        >
          Recentst gestartte etappe
        </button>
        <button
          style={{ marginRight: "5px", width: "150px" }}
          onClick={() => updateResult({ aankomende: true })}
        >
          Aankomende etappe
        </button>
      </div>
      <div style={{ marginTop: "5px" }}>
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={year2}
          onChange={(e) => setYear2(e.target.value)}
          placeholder="year"
        />
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={raceName2}
          onChange={(e) => setRace2(e.target.value)}
          placeholder="racename"
        />
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={raceId}
          onChange={(e) => setRaceId(e.target.value)}
          placeholder="raceId"
        />
        <button
          style={{ marginRight: "5px", width: "110px" }}
          onClick={() => updateStartlist({ raceName2, year2, raceId })}
        >
          Update Startlist
        </button>
        <button
          style={{ marginRight: "5px", width: "130px" }}
          onClick={() => etappesToevoegen({ raceId })}
        >
          Etappes toevoegen
        </button>
        <button
          style={{ marginRight: "5px", width: "160px" }}
          onClick={() => downloadStageProfiles({ raceId })}
        >
          Download stage profiles
        </button>
        <button
          style={{ marginRight: "5px", width: "100px" }}
          onClick={() => raceFinished({ raceName2, year2, raceId })}
        >
          Race Finished
        </button>
      </div>
      <div style={{ marginTop: "10px" }}>
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={statsRaceId}
          onChange={(e) => setStatsRaceId(e.target.value)}
          placeholder="raceId"
        />
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={statsStageNr}
          onChange={(e) => setStatsStageNr(e.target.value)}
          placeholder="stage?"
        />
        <button
          style={{ marginRight: "5px", width: "220px" }}
          onClick={() =>
            calculateStageSelectionStats({
              raceId: statsRaceId,
              ...(statsStageNr.trim() !== "" ? { stagenr: statsStageNr } : {}),
            })
          }
        >
          Calculate StageSelection stats
        </button>
      </div>
      <div style={{ marginTop: "10px" }}>
        <input
          style={{ marginRight: "5px", width: "70px" }}
          type="text"
          value={apStatsRaceId}
          onChange={(e) => setApStatsRaceId(e.target.value)}
          placeholder="raceId"
        />
        <button
          style={{ marginRight: "5px", width: "260px" }}
          onClick={() => calculateAccountParticipationStats({ raceId: apStatsRaceId })}
        >
          Calculate AccountParticipation stats
        </button>
      </div>
      <div style={{ marginTop: "10px" }}>
        <button style={{ marginRight: "5px", width: "150px" }} onClick={resetCache}>
          Reset Cache
        </button>
        <button style={{ marginRight: "5px", width: "150px" }} onClick={playwrightDiag}>
          Playwright Diag
        </button>
      </div>
      {diag && (
        <pre
          style={{
            marginTop: "15px",
            padding: "10px",
            border: "1px solid #888",
            background: "#f4f4f4",
            fontSize: "12px",
            whiteSpace: "pre-wrap",
          }}
        >
          {JSON.stringify(diag, null, 2)}
        </pre>
      )}
      {error && (
        <div
          style={{
            marginTop: "15px",
            padding: "10px",
            border: "1px solid #c00",
            background: "#fee",
            color: "#900",
            fontFamily: "monospace",
            whiteSpace: "pre-wrap",
            fontSize: "12px",
          }}
        >
          {typeof error === "string" ? (
            error
          ) : (
            <>
              <div><strong>{error.type}</strong>: {error.message}</div>
              {error.inner && <div style={{ marginTop: "8px" }}>Inner: {error.inner}</div>}
              {error.stackTrace && <div style={{ marginTop: "8px" }}>{error.stackTrace}</div>}
            </>
          )}
        </div>
      )}
    </div>
  );
};

export default Admin;
