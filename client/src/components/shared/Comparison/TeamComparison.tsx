// import axios from "axios";
import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../BudgetContextProvider";
import TeamComparisonTable from "./TeamComparisonTable";
import { UserSelection } from "../../../models/UserSelection";
import AllSelectedRiders, { AllSelectedRiderRow } from "./AllSelectedRidersTable";

const TeamComparison = ({ raceId, stagenr }: { raceId: string, stagenr?: string }) => {
    const budgetParticipation = useBudgetContext();
    const [teams, setTeams] = useState<UserSelection[]>([]);
    const [allRiders, setAllRiders] = useState<AllSelectedRiderRow[]>([]);

    useEffect(() => {
        if (stagenr) { // delay zodat de rest van stage result sneller geladen wordt
            setTimeout(() => {
                axios
                    .get(`/api/stageresult/comparison`, { params: { raceId, stagenr, budgetParticipation } })
                    .then(res => {
                        setTeams(res.data.teams);
                        setAllRiders(res.data.counts);
                    })
                    .catch(error => {
                        throw error;
                    });
            }, 500);
        } else {
            axios
                .get(`/api/race/comparison`, { params: { raceId, stagenr, budgetParticipation } })
                .then(res => {
                    setTeams(res.data.teams);
                    setAllRiders(res.data.counts);
                })
                .catch(error => {
                    throw error;
                });
        }
    }, [raceId, stagenr, budgetParticipation]);

    return (
        <div >
            {<div style={{ display: 'flex', flexWrap: 'wrap' }}>
                {teams.map((userSelection, index) => (
                    <div key={index} style={{ flex: '0 0 24%', marginRight: '2px', marginBottom: '2px' }}>
                        <TeamComparisonTable key={index} title={userSelection.username} riders={userSelection.riders} />
                        <div style={{ marginTop: '2px' }}>
                            {userSelection.gemist.length > 0 && <TeamComparisonTable key={index} title={"Niet Opgesteld"} riders={userSelection.gemist} />}
                        </div>
                    </div>
                ))}
            </div>}
            <AllSelectedRiders riders={allRiders} />
        </div>
    )
}

export default TeamComparison;