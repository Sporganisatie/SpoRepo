import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";

const JoinRace = () => {
    document.title = "Race Deelname";
    let navigate = useNavigate();
    let { raceId } = useParams();
    const handleJoinRace = () => {
        axios.get(`/api/Race/join`, { params: { raceId } })
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
