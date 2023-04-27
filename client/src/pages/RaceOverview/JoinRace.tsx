import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";

const JoinRace = () => {
    let navigate = useNavigate();
    let { raceId } = useParams();
    const handleJoinRace = () => {
        axios.post(`/api/race/${raceId}/join`)
            .then(res => {
                navigate(`/${raceId}/teamselection`);
            })
            .catch(err => {
                console.error(err);
            });
    }

    return (
        <div>
            <button onClick={handleJoinRace}>Join Race</button>
        </div>
    );
}

export default JoinRace;
