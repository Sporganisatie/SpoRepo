import { useEffect, useState } from 'react';
import Select, { SelectOption } from '../../Select';
import { useRaceContext, useRaceDispatch } from '../../shared/RaceContextProvider';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';

const RaceDropdown = () => {
    const raceDispatch = useRaceDispatch();
    const navigate = useNavigate();
    const race = useRaceContext();
    const location = useLocation();
    const [raceOptions, setRaceOptions] = useState<SelectOption<number>[]>();

    useEffect(() => {
        axios
            .get(`/api/race/all`)
            .then((res) => {
                setRaceOptions(res.data);
            })
    }, []);

    function handleOnchange(selectedOption: number) {
        raceDispatch(selectedOption);
        const match = location.pathname.match(/\d+(.*)/);

        if (match) {
            navigate(`/${selectedOption}${match[1]}`);
        } else {
            raceDispatch(selectedOption);
        }
    }

    return (
        <Select<number>
            value={race}
            options={raceOptions ?? []}
            onChange={(selectedOption: number) => { handleOnchange(selectedOption) }}
        />
    );
};

export default RaceDropdown;