import TeamComparisonTable from "./TeamComparisonTable";
import { useTeamComparison } from "./TeamComparisonHook";

const TeamComparison = () => {
  const { data, isFetching } = useTeamComparison();

  return (
    <div>
      {
        <div style={{ display: "flex", flexWrap: "wrap" }}>
          {data.map((userSelection, index) => (
            <div
              key={index}
              style={{
                flex: "0 0 24%",
                marginRight: "2px",
                marginBottom: "2px",
              }}
            >
              <TeamComparisonTable
                key={index}
                title={userSelection.username}
                riders={userSelection.riders}
              />
              <div style={{ marginTop: "2px" }}>
                {userSelection.gemist.length > 0 && (
                  <TeamComparisonTable
                    key={index}
                    title={"Niet Opgesteld"}
                    riders={userSelection.gemist}
                  />
                )}
              </div>
            </div>
          ))}
        </div>
      }
    </div>
  );
};

export default TeamComparison;
