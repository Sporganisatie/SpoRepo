import { useState } from "react";
import axios from "axios";

const Admin = () => {
  const [todo, setTodo] = useState("");

  const handleSubmit = () => {
    axios.get("/api/testdata")
      .then(res => {
        console.log("Todo submitted successfully!");
        setTodo("");
      })
      .catch(err => {
        console.error(err);
      });
  };

  return (
    <div>
      <input
        type="text"
        value={todo}
        onChange={e => setTodo(e.target.value)}
        placeholder="stage"
      />
      <button onClick={handleSubmit}>Uitslag scrape</button>
    </div>
  );
};

export default Admin;
