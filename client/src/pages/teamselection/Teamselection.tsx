import axios from 'axios';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { SelectableRider } from './Models/SelectableRider';
import { TeamSelectionData } from './Models/TeamSelectionData';
import SelectableRidersTable from './SelectableRidersTable';
import TeamSelectionTable from './TeamSelectionTable';
import { RiderParticipation } from '../../models/RiderParticipation';
import { useBudgetContext } from '../../components/shared/BudgetContextProvider';
import FilterElements, { Filters } from './Filters';

const Teamselection: React.FC = () => {
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<TeamSelectionData>({ budget: 0, budgetOver: 0, team: [], allRiders: [] });
    const [pending, setPending] = useState(true);
    const [filteredRiders, setFilteredRiders] = useState<SelectableRider[]>([]);
    const [filters, setFilters] = useState(getDefaulFilterState())
    function getDefaulFilterState(): Filters {
        return {
            name: "",
            minPrice: 500000,
            maxPrice: 7000000,
            team: "",
            skill: ""
        }
    }
    function updateAndFilter(part: Partial<Filters>) {
        const newFilter = {
            ...filters,
            ...part
        }
        newFilter.minPrice = Math.min(newFilter.minPrice, newFilter.maxPrice)
        newFilter.maxPrice = Math.max(newFilter.minPrice, newFilter.maxPrice)
        setFilters(newFilter)
        setFilteredRiders(filterRiders(newFilter, data.allRiders));
    }

    /* eslint-disable */
    useEffect(() => loadData(), [raceId, budgetParticipation])
    useEffect(() => { updateAndFilter({}); setPending(false); }, [data])
    /* eslint-enable */
    const loadData = () => {
        axios.get(`/api/TeamSelection`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data)
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
                budgetParticipation
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
                budgetParticipation
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
            <FilterElements updateFilter={updateAndFilter} resetFilter={resetFilter} filters={filters} />
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
                riders = riders.filter(({ details }) => details.price >= value);
                break;
            case "maxPrice":
                riders = riders.filter(({ details }) => details.price <= value);
                break;
            case "team":
                riders = riders.filter(({ details }) => details.team.toLowerCase().includes(value.toLowerCase()));
                break;
            case "skill":
                riders = riders.filter(({ details }) => SkillFilter(details, value))
                riders.sort((A, B) => SkillSort(A, B, value))
        }
    }
    return riders;
}

function SkillFilter(details: RiderParticipation, value: any): unknown {
    switch (value) {
        case "gc":
            return details.gc > 0;
        case "sprint":
            return details.sprint > 0;
        case "climb":
            return details.climb > 0;
        case "tt":
            return details.tt > 0;
        case "punch":
            return details.punch > 0;
        default:
            return true;
    }
}
function SkillSort(A: SelectableRider, B: SelectableRider, value: any): number {
    switch (value) {
        case "gc":
            return B.details.gc - A.details.gc;
        case "sprint":
            return B.details.sprint - A.details.sprint;
        case "climb":
            return B.details.climb - A.details.climb;
        case "tt":
            return B.details.tt - A.details.tt;
        case "punch":
            return B.details.punch - A.details.punch;
        default:
            return B.details.price - A.details.price;
    }
}

