import { useState } from "react";
import axios from "axios";

const Admin = () => {
  document.title = "Admin";
  const [stagenr, setStagenr] = useState("");
  const [year, setYear] = useState("2023");
  const [raceName, setRace] = useState("vuelta");

  const submit = (params: any) => {
    axios.get(`/api/Admin/stageResults`, { params })
      .then(res => {

      })
      .catch(err => {
        console.error(err);
      });
  }

  return (
    <div>
      <input
        type="text"
        value={stagenr}
        onChange={e => setStagenr(e.target.value)}
        placeholder="stage"
      />
      <input
        type="text"
        value={raceName}
        onChange={e => setRace(e.target.value)}
        placeholder="race"
      />
      <input
        type="text"
        value={year}
        onChange={e => setYear(e.target.value)}
        placeholder="year"
      />
      <button onClick={() => submit({ raceName, year, stagenr })}>Uitslag scrape</button>
      <button onClick={() => submit({ mostRecent: true })}>Recentste etappe</button>
    </div>
  );
};

export default Admin;
