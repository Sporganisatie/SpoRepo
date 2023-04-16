import axios from 'axios';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { SelectableRider } from './Models/SelectableRider';
import SelectableRidersTable from './SelectableRidersTable';

const Teamselection: React.FC = () => {
    let { raceId } = useParams();
    const [data, setData] = useState<SelectableRider[]>([]);
    const [pending, setPending] = useState(true);

    useEffect(() => loadData(), [])

    const loadData = () => {
        axios.get(`/api/TeamSelection`, { params: { raceId } })
            .then(res => {
                setData(res.data.allRiders)
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
        <SelectableRidersTable
            data={data}
            loading={pending}
            removeRider={removeRider}
            addRider={addRider}
        />
    );
}

export default Teamselection;
