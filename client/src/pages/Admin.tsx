import { useState } from "react";
import axios from "axios";

const Admin = () => {
  document.title = "Admin";
  const [stagenr, setStagenr] = useState("");
  const [year, setYear] = useState("");
  const [raceName, setRace] = useState("");

  const [raceId, setRaceId] = useState("");
  const [year2, setYear2] = useState("");
  const [raceName2, setRace2] = useState("");

  const updateResult = (params: any) => {
    axios.get(`/api/Admin/stageResults`, { params })
      .then(_ => { })
      .catch(err => { console.error(err); });
  }

  const updateStartlist = (params: any) => {
    axios.get(`/api/Admin/startlist`, { params })
      .then(_ => { })
      .catch(err => { console.error(err); });
  }

  const raceFinished = (params: any) => {
    axios.get(`/api/Admin/RaceFinished`, { params })
      .then(_ => { })
      .catch(err => { console.error(err); });
  }

  const etappesToevoegen = (params: any) => {
    axios.get(`/api/Admin/AddStages`, { params })
      .then(_ => { })
      .catch(err => { console.error(err); });
  }

  const resetCache = () => {
    axios.get(`/api/Admin/resetCache`)
      .then(_ => { })
      .catch(err => { console.error(err); });
  };

  return (
    <div>
      <div>
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={stagenr}
          onChange={e => setStagenr(e.target.value)}
          placeholder="stage"
        />
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={raceName}
          onChange={e => setRace(e.target.value)}
          placeholder="racename"
        />
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={year}
          onChange={e => setYear(e.target.value)}
          placeholder="year"
        />
        <button style={{ marginRight: '5px', width: '110px' }} onClick={() => updateResult({ raceName, year, stagenr })}>Uitslag scrape</button>
        <button style={{ marginRight: '5px', width: '180px' }} onClick={() => updateResult({ mostRecentStarted: true })}>Recentst gestartte etappe</button>
        <button style={{ marginRight: '5px', width: '150px' }} onClick={() => updateResult({ aankomende: true })}>Aankomende etappe</button>
      </div>
      <div style={{ marginTop: '5px' }} >
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={year2}
          onChange={e => setYear2(e.target.value)}
          placeholder="year"
        />
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={raceName2}
          onChange={e => setRace2(e.target.value)}
          placeholder="racename"
        />
        <input style={{ marginRight: '5px', width: '70px' }}
          type="text"
          value={raceId}
          onChange={e => setRaceId(e.target.value)}
          placeholder="raceId"
        />
        <button style={{ marginRight: '5px', width: '110px' }} onClick={() => updateStartlist({ raceName2, year2, raceId })}>Update Startlist</button>
        <button style={{ marginRight: '210px', width: '130px' }} onClick={() => etappesToevoegen({ raceId })}>Etappes toevoegen</button>
        <button style={{ marginRight: '5px', width: '100px' }} onClick={() => raceFinished({ raceName2, year2, raceId })}>Race Finished</button>
      </div>
      <div style={{ marginTop: '10px' }}>
        <button style={{ marginRight: '5px', width: '150px' }} onClick={resetCache}>
          Reset Cache
        </button>
      </div>
    </div>
  );
};

export default Admin;
