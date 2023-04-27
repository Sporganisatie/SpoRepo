import axios from "axios";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

// TODO dit component vervangen door pure typescript
const RaceRouter = () => {
    let navigate = useNavigate();
    let { raceId } = useParams();
    var a = "";
    useEffect(() => {
        axios.get(`/api/Race`, { params: { raceId } })
            .then(res => {
                switch (res.data) {
                    case RaceStateEnum.NotJoined: navigate(`/joinrace/${raceId}`); return;
                    case RaceStateEnum.TeamSelection: navigate(`/${raceId}/teamselection`); return;
                };
                a = "Dit moet je niet kunnen zien val Arjen lastig als je dit wel ziet";
                // TODO wat als invalid race
            })
            .catch(function (error) {
                throw error
            });
    }, [raceId])

    return (
        <div>
            {a}
        </div>
    )
}

enum RaceStateEnum {
    None,
    NotJoined,
    TeamSelection,
    Started,
    Finished
}

export default RaceRouter;
