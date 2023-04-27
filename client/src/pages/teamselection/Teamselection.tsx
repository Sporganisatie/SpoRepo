import axios from 'axios';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { SelectableRider } from './Models/SelectableRider';
import { TeamSelectionData } from './Models/TeamSelectionData';
import SelectableRidersTable from './SelectableRidersTable';
import TeamSelectionTable from './TeamSelectionTable';

interface Filters {
    name: string,
    minPrice: string,
    maxPrice: string,
    team: string,
};

const Teamselection: React.FC = () => {
    let { raceId } = useParams();
    const [data, setData] = useState<TeamSelectionData>({ budget: 0, budgetOver: 0, team: [], allRiders: [] });
    const [pending, setPending] = useState(true);
    const [filteredRiders, setFilteredRiders] = useState<SelectableRider[]>([]);
    const [filters, setFilters] = useState(getDefaulFilterState())
    function getDefaulFilterState(): Filters {
        return {
            name: "",
            minPrice: "500000",
            maxPrice: "7000000",
            team: "",
        }
    }
    function updateAndFilter(part: Partial<Filters>) {
        const newFilter = {
            ...filters,
            ...part
        }
        setFilters(newFilter)
        setFilteredRiders(filterRiders(newFilter, data.allRiders));
    }


    useEffect(() => loadData(), [raceId])

    const loadData = () => {
        axios.get(`/api/TeamSelection`, { params: { raceId } })
            .then(res => {
                setData(res.data)
                setFilteredRiders(res.data.allRiders);
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

    const resetFilter = () => {
        updateAndFilter(getDefaulFilterState());
    }

    return (
        <div style={{ display: "flex", flexDirection: "column" }}>
            <div>Budget Over: {data.budgetOver / 1_000_000}M/{data.budget / 1_000_000}M</div>
            <div style={{ display: "flex", flexDirection: "row", gap: "0.5rem", margin: "1rem" }}>
                <input
                    type="text"
                    value={filters.name}
                    placeholder="naam"
                    onChange={(e) => updateAndFilter({ name: e.target.value })}
                />
                <select value={filters.minPrice} onChange={(e) => {
                    const maxPrice = Math.max(Number(e.target.value), Number(filters.maxPrice));
                    updateAndFilter({ minPrice: e.target.value, maxPrice: maxPrice.toString() });
                }}>
                    <option value="500000">500.000</option>
                    <option value="750000">750.000</option>
                    <option value="1000000">1.000.000</option>
                    <option value="1500000">1.500.000</option>
                    <option value="2000000">2.000.000</option>
                    <option value="2500000">2.500.000</option>
                    <option value="3000000">3.000.000</option>
                    <option value="3500000">3.500.000</option>
                    <option value="4000000">4.000.000</option>
                    <option value="4500000">4.500.000</option>
                    <option value="5000000">5.000.000</option>
                    <option value="5500000">5.500.000</option>
                    <option value="6000000">6.000.000</option>
                    <option value="7000000">7.000.000</option>
                </select>
                <select value={filters.maxPrice} onChange={(e) => {
                    const minPrice = Math.min(Number(e.target.value), Number(filters.minPrice));
                    updateAndFilter({ maxPrice: e.target.value, minPrice: minPrice.toString() });
                }}>
                    <option value="500000">500.000</option>
                    <option value="750000">750.000</option>
                    <option value="1000000">1.000.000</option>
                    <option value="1500000">1.500.000</option>
                    <option value="2000000">2.000.000</option>
                    <option value="2500000">2.500.000</option>
                    <option value="3000000">3.000.000</option>
                    <option value="3500000">3.500.000</option>
                    <option value="4000000">4.000.000</option>
                    <option value="4500000">4.500.000</option>
                    <option value="5000000">5.000.000</option>
                    <option value="5500000">5.500.000</option>
                    <option value="6000000">6.000.000</option>
                    <option value="7000000">7.000.000</option>
                </select>
                <input
                    type="text"
                    placeholder="teamnaam"
                    value={filters.team}
                    onChange={(e) => updateAndFilter({ team: e.target.value })}
                />
                <button onClick={() => resetFilter()}>
                    Reset
                </button>
            </div>
            <div style={{ display: "flex" }}>
                <SelectableRidersTable
                    data={filteredRiders}
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
        </div>
    );
}

export default Teamselection;

function filterRiders(filters: Filters, riders: SelectableRider[]): SelectableRider[] {
    for (const [filterType, value] of Object.entries(filters)) {
        if (!value) {
            continue;
        }
        switch (filterType) {
            case "name":
                riders = riders.filter(({ details }) =>
                    (details.rider.firstname + details.rider.lastname).toLowerCase().includes(value.toLowerCase())
                );
                break;
            case "minPrice":
                riders = riders.filter(({ details }) => details.price >= Number(value));
                break;
            case "maxPrice":
                riders = riders.filter(({ details }) => details.price <= Number(value));
                break;
            case "team":
                riders = riders.filter(({ details }) => details.team.toLowerCase().includes(value.toLowerCase()));
                break;
        }
    }
    return riders;
}