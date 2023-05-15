import { useParams } from "react-router-dom";
import TeamComparison from "../../components/shared/Comparison/TeamComparison";

const TeamComparisonPage = () => {
    let { raceId } = useParams();
    document.title = "Alle teams"
    return (
        <TeamComparison raceId={raceId ?? ""} />
    )
}

export default TeamComparisonPage;