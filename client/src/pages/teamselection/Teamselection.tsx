import axios from 'axios';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { TeamSelectionData } from './Models/TeamSelectionData';
import SelectableRidersTable from './SelectableRidersTable';
import TeamSelectionTable from './TeamSelectionTable';

const Teamselection: React.FC = () => {
    let { raceId } = useParams();
    const [data, setData] = useState<TeamSelectionData>({ budget: 0, budgetOver: 0, team: [], allRiders: [] });
    const [pending, setPending] = useState(true);

    useEffect(() => loadData(), [])

    const loadData = () => {
        axios.get(`/api/TeamSelection`, { params: { raceId } })
            .then(res => {
                setData(res.data)
                setPending(false);
            })
            .catch(function (error) {
                throw error
            });
    };

    const removeRider = (riderParticipationId: number) => {
        setPending(true);
        axios.delete('/api/TeamSelection', {
            params: {
                riderParticipationId,
                raceId,
                budgetParticipation: false
            }
        })
            .then(function (response) {
                loadData();
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    const addRider = (riderParticipationId: number) => {
        setPending(true);
        axios.post('/api/TeamSelection', null, {
            params: {
                riderParticipationId,
                raceId,
                budgetParticipation: false
            }
        })
            .then(function (response) {
                loadData();
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    return (
        <div style={{ display: "flex" }}>
            <div>Budget Over: {data.budgetOver / 1_000_000}M/{data.budget / 1_000_000}M</div>
            <SelectableRidersTable
                data={data.allRiders}
                loading={pending}
                removeRider={removeRider}
                addRider={addRider}
            />
            <TeamSelectionTable
                data={data.team}
                loading={pending}
                removeRider={removeRider}
            />
        </div>
    );
}

export default Teamselection;
