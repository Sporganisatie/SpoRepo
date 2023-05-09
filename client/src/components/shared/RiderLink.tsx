import { Link } from "react-router-dom";
import { Rider } from "../../models/Rider";

const RiderLink = ({ rider, kopman }: { rider: Rider, kopman?: boolean }) =>
    <Link className="tableLink" to={"/rider/" + rider.riderId}>
        {(kopman ? "*" : "") + rider.firstname + " " + rider.lastname}
    </Link>

export default RiderLink