import { useState } from "react";
import AllSelectedRiders from "./AllSelectedRidersTable";
import { useTeamComparison } from "./TeamComparisonHook";
import TeamComparisonUser from "./TeamComparisonUser";
import SmallSwitch from "../SmallSwitch";

const TeamComparison = () => {
    const [toggles, setToggles] = useState<{ username: string, showUser: boolean }[]>([]);
    const { data, toggleUser, toggleAll } = useTeamComparison(setToggles);

    return (
        <div>
            {toggles.map((user, index) => (
                <SmallSwitch key={index} text={user.username} selected={user.showUser} index={index} toggleUser={() => toggleUser(index)} />
            ))}
            <div style={{ display: 'inline-block', marginLeft: "5px", marginBottom: "5px" }}>
                <button onClick={toggleAll}>Toggle alle</button>
            </div>
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
