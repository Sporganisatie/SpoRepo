import axios from "axios";
import { useState } from "react";
import { useParams } from "react-router-dom";
import Table from "../components/Table";

const StageResultTest = () => {
    let { raceid, stagenr } = useParams();
    const [data, setData] = useState([]);
    const retrieveData = () => {
        axios.get(`/api/stage/${raceid}/${stagenr}/teamresults`)
            .then(res => {
                setData(res.data.value)
            })
            .catch(function (error) {
                throw error
            });
    }

    return (
        <div>
            <Table headers={["first", "second"]} data={data} />
            {data}
            <button onClick={() => retrieveData()}>Data ophalen</button>
        </div>);
};

export default StageResultTest;
