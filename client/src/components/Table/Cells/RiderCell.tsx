import { Link } from "react-router-dom";
import { Rider } from "../../../models/Rider";

interface RiderCellProps {
    rider: Rider,
}

const RiderCell = (props: RiderCellProps) =>
    <td>
        <Link className="tableLink" to={"/rider/" + props.rider.riderId}>
            {props.rider.firstName + " " + props.rider.lastName}
        </Link>
    </td>

export default RiderCell