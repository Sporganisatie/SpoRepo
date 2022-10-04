import axios from "axios";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Table from "../components/Table/Table";
import { Rider } from "../models/Rider";

var headers = [
    { title: "", name: "stagePosition" },
    { title: "Points", name: "stagePoints" },
    { title: "Rider", name: "rider" }
]

interface TeamResultRow {
    rider: Rider;
    stagePosition: number
    stagePoints: number
}

const StageResultTest = () => {
    let { raceid, stagenr } = useParams();
    const [data, setData] = useState<TeamResultRow[]>([]);
    useEffect(() => {
        axios.get(`/api/stage/${raceid}/${stagenr}/teamresults`)
            .then(res => {
                setData(res.data.value)
            })
            .catch(function (error) {
                throw error
            });
    }, [raceid, stagenr])

    return (<Table headers={headers} data={data} />);
};

export default StageResultTest;
