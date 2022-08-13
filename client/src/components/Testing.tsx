import axios from "axios";
import { useState } from "react";

const Testing = () => {
  const [data, setData] = useState('Als je dit ziet dan is er nog geen call geweest');
  const [query, setQuery] = useState('');

  const retrieveData = () => {
    axios.get('testdata')
      .then(res => {
        setData(res.data)
      })
      .catch(function (error) {
        throw error
      });
  }

  const sqlcall = () => {
    console.log("sqlcall")
    axios.post('testdata/sql', query, { headers: { 'Content-Type': 'application/json' } })
      .then(res => {
        console.log(res)
      })
      .catch(function (error) {
        throw error
      });
  }

  return (
    <div>
      {data}
      <button onClick={() => retrieveData()}>Data ophalen</button>
      <br />
      <input
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="query" />
      <button onClick={() => sqlcall()}>Data ophalen</button>
    </div>);
};

export default Testing;
