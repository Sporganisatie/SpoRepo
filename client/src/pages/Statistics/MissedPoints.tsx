import { useParams } from "react-router-dom";

const MissedPoints = () => {
    let { raceId } = useParams();
    // make api call
    // set data
    return (
        // put data into a DataTable
        <div>Missed points voor {raceId}</div>
    )
}

export default MissedPoints;