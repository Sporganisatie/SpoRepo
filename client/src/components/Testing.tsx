import axios from "axios";
import { useState } from "react";

const Testing = () => {
  const [data, setData] = useState('Als je dit ziet dan is er nog geen call geweest');

  const retrieveData = () => {
    axios.get('testdata')
      .then(res => {
        setData(res.data)
      })
      .catch(function (error) {
        throw error
      });
  }

  return (
    <div>
      {data}
      <button onClick={() => retrieveData()}>Data ophalen</button>
    </div>);
};

export default Testing;
