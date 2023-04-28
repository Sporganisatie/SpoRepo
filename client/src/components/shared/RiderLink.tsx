import { Link } from "react-router-dom";
import { Rider } from "../../models/Rider";

const RiderLink = ({ rider }: { rider: Rider }) =>
    <Link className="tableLink" to={"/rider/" + rider.riderId}>
        {rider.firstname + " " + rider.lastname}
    </Link>

export default RiderLink