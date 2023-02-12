import { useEffect, useState } from "react";
import TeamselectionRepository from "../../repositories/teamselectionRepository";

function Teamselection() {
    const [names, setNames] = useState<string[]|undefined>();

    async function getRiderNames() {
        const repository = new TeamselectionRepository();
        const riders = await repository.getAll(26);
        setNames(riders.map(({rider}) => rider.lastName))
    }
    
    if (!names) {
        getRiderNames();
    }
    
    return (
        <div>
            { names }
        </div>
    )
}

export default Teamselection;
