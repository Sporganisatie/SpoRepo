import { useParams } from "react-router-dom";

const MissedPoints = () => {
    let { raceId } = useParams();
    return (
        <div>Missed points voor {raceId}</div>
    )
}

export default MissedPoints;