import { useParams } from "react-router-dom";
import TeamComparison from "./TeamComparison";

const TeamComparisonPage = () => {
    let { raceId } = useParams();
    return (
        <TeamComparison raceId={raceId ?? ""} />
    )
}

export default TeamComparisonPage;