import { useState } from "react";
import AllSelectedRiders from "./AllSelectedRidersTable";
import { useTeamComparison } from "./TeamComparisonHook";
import TeamComparisonUser from "./TeamComparisonUser";

const TeamComparison = () => {
    const [toggles, setToggles] = useState<{ username: string, showUser: boolean }[]>([]);
    const { data, handleToggle, toggleAll } = useTeamComparison(setToggles);

    return (
        <div>
            {toggles.map((user, index) => (
                <div key={index} style={{ display: 'inline-block', cursor: 'pointer' }} onClick={() => handleToggle(index)}>
                    <input type="checkbox" checked={user.showUser} onChange={() => { }} />
                    {user.username}
                </div>
            ))}
            <button onClick={toggleAll}>Toggle alle</button>
            <div style={{ display: "flex", flexWrap: "wrap" }}>
                {data?.teams.filter((_, index) => toggles[index].showUser).map((userSelection, index) => (
                    <TeamComparisonUser key={index} userSelection={userSelection} />
                ))}
                <AllSelectedRiders riders={data?.counts ?? []} />
            </div>
        </div>
    );
};

export default TeamComparison;
