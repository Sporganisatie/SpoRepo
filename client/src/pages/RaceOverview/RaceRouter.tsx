import axios from "axios";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

enum RaceStateEnum {
    None,
    NotJoined,
    TeamSelection,
    Started,
    Finished
}

// TODO dit component vervangen door pure typescript
const RaceRouter = () => {
    let navigate = useNavigate();
    let { raceId } = useParams();
    useEffect(() => {
        axios.get(`/api/Race`, { params: { raceId } })
            .then(res => {
                switch (res.data) {
                    case RaceStateEnum.NotJoined: navigate(`/joinrace/${raceId}`); return;
                    case RaceStateEnum.TeamSelection: navigate(`/teamselection/${raceId}`); return;
                };
                // TODO wat als invalid race
            })
            .catch(function (error) {
                throw error
            });
    }, [raceId, navigate])

    return (
        <div>
            Dit moet je niet kunnen zien val Arjen lastig als je dit wel ziet
        </div>
    )
}

export default RaceRouter;
