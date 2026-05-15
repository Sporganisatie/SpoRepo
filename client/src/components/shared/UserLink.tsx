import { Link } from "react-router-dom";

const UserLink = ({ username }: { username: string }) => (
    <Link className="tableLink" to={`/profile/${encodeURIComponent(username)}`}>
        {username}
    </Link>
);

export default UserLink;
