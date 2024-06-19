import { UserSelection } from "../../../models/UserSelection";
import TeamComparisonTable from "./TeamComparisonTable";

const TeamComparisonUser = ({ userSelection }: { userSelection: UserSelection }) => {
    return (
        <div style={{ flex: "0 0 24%", marginRight: "2px", marginBottom: "2px", }}>
            <TeamComparisonTable title={userSelection.username} riders={userSelection.riders} />
            <div style={{ marginTop: "2px" }}>
                {userSelection.gemist.length > 0 && (<TeamComparisonTable title={"Niet Opgesteld"} riders={userSelection.gemist} />)}
            </div>
        </div>
    )
}

export default TeamComparisonUser;