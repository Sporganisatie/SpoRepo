import TeamOverlapTables from "./TeamOverlapTables";

const TeamOverlap = () => {
    document.title = "Team Overlap";
    return (
        <div >
            <TeamOverlapTables includeDnfRiders={true} />
            <TeamOverlapTables includeDnfRiders={false} />
        </div>
    )
}

export default TeamOverlap;