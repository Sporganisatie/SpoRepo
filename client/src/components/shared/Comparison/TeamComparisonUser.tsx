import type { UserSelection } from "../../../models/UserSelection";
import TeamComparisonTable from "./TeamComparisonTable";

const TeamComparisonUser = ({ userSelection }: { userSelection: UserSelection }) => {
  return (
    <div style={{ flex: "0 0 24%" }}>
      <div className="tc-user-name">
        <TeamComparisonTable title={userSelection.username} riders={userSelection.riders} />
      </div>
      <div style={{ marginTop: "0.5rem", marginBottom: "1rem" }}>
        {userSelection.gemist.length > 0 && (
          <TeamComparisonTable title={"Niet Opgesteld"} riders={userSelection.gemist} />
        )}
      </div>
    </div>
  );
};

export default TeamComparisonUser;
