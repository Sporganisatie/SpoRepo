import AllSelectedRiders from "./AllSelectedRidersTable";
import { useTeamComparison } from "./TeamComparisonHook";
import TeamComparisonUser from "./TeamComparisonUser";

const TeamComparison = () => {
    const { data, handleToggle } = useTeamComparison();
    return (
        <div>
            {data?.teams.map((userSelection, index) => (
                <div key={index} style={{ display: 'inline-block', cursor: 'pointer' }} onClick={() => handleToggle(index)}>
                    <input type="checkbox" checked={!userSelection.hideUser} onChange={() => { }} />
                    {userSelection.username}
                </div>
            ))}
            {/* <button onClick={() => toggleAnderen}>Toggle anderen</button> */}
            <div style={{ display: "flex", flexWrap: "wrap" }}>
                {data?.teams.filter(x => !x.hideUser).map((userSelection, index) => (
                    <TeamComparisonUser key={index} userSelection={userSelection} />
                ))}
                <AllSelectedRiders riders={data?.counts ?? []} />
            </div>
        </div>
    );
};

export default TeamComparison;
