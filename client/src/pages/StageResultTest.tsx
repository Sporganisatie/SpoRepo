import axios from "axios";
import { useState } from "react";
import { useParams } from "react-router-dom";

const StageResultTest = () => {
    let { raceid, stagenr } = useParams();
    const [data, setData] = useState('Als je dit ziet dan is er nog geen call geweest' + raceid + " " + stagenr);
    const retrieveData = () => {
        axios.get(`/api/stage/${raceid}/${stagenr}/teamresults`)
            .then(res => {
                setData(res.data.value[0].lastname)
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

export default StageResultTest;
