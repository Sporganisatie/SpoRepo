import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";

const JoinRace = () => {
    let navigate = useNavigate();
    let { raceid } = useParams();
    const handleJoinRace = () => {
        axios.get(`/api/Race/join`, { params: { raceId: raceid } })
            .then(res => {
                navigate(`/${raceid}/teamselection`);
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
