// import axios from "axios";
import axios from "axios";
import { useEffect, useState } from "react";
import { useBudgetContext } from "../BudgetContextProvider";
import TeamComparisonTable from "./TeamComparisonTable";
import { UserSelection } from "../../../models/UserSelection";

const TeamComparison = ({ raceId, stagenr }: { raceId: string, stagenr?: string }) => {
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<UserSelection[]>([]);

    useEffect(() => {
        if (stagenr) { // delay zodat de rest van stage result sneller geladen wordt
            setTimeout(() => {
                axios
                    .get(`/api/stageresult/comparison`, { params: { raceId, stagenr, budgetParticipation } })
                    .then(res => {
                        setData(res.data);
                    })
                    .catch(error => {
                        throw error;
                    });
            }, 500);
        }
    }, [raceId, stagenr, budgetParticipation]);

    return (
        <div >
            {<div style={{ display: 'flex', flexWrap: 'wrap' }}>
                {data.map((userSelection, index) => (
                    <div key={index} style={{ flex: '0 0 24%', marginRight: '2px', marginBottom: '2px' }}>
                        <TeamComparisonTable key={index} title={userSelection.username} riders={userSelection.riders} />
                        <div style={{ marginTop: '2px' }}>
                            {userSelection.gemist.length > 0 && <TeamComparisonTable key={index} title={"Niet Opgesteld"} riders={userSelection.gemist} />}
                        </div>
                    </div>
                ))}
            </div>}
        </div>
    )
}

export default TeamComparison;