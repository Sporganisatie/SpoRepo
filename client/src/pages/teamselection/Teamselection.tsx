import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { SelectableRider } from "./Models/SelectableRider";
import SelectableRidersTable from "./SelectableRidersTable";
import TeamSelectionTable from "./TeamSelectionTable";
import { RiderParticipation } from "../../models/RiderParticipation";
import FilterElements, { Filters } from "./Filters";
import RiderTypeTotals from "./RiderTypeTotals";
import "./teamSelection.css";
import { useTeamSelection } from "./TeamSelectionHook";
import { useRaceContext } from "../../components/shared/RaceContextProvider";

const TeamSelection: React.FC = () => {
    document.title = "Team Selectie";
    let navigate = useNavigate();
    const raceId = useRaceContext();
    const { data, isLoading, addRider, removeRider } = useTeamSelection();
    const [filteredRiders, setFilteredRiders] = useState<SelectableRider[]>([]);
    const [filters, setFilters] = useState(getDefaulFilterState());
    function getDefaulFilterState(): Filters {
        return {
            name: "",
            minPrice: 500000,
            maxPrice: 8000000,
            team: "",
            skill: "",
        };
    }
    function updateAndFilter(part: Partial<Filters>) {
        const newFilter = {
            ...filters,
            ...part,
        };
        newFilter.minPrice = Math.min(newFilter.minPrice, newFilter.maxPrice);
        newFilter.maxPrice = Math.max(newFilter.minPrice, newFilter.maxPrice);
        setFilters(newFilter);
        setFilteredRiders(filterRiders(newFilter, data?.allRiders ?? []));
    }

    /* eslint-disable */
    useEffect(() => {
        updateAndFilter({});
    }, [data]);
    /* eslint-enable */

    const resetFilter = () => {
        updateAndFilter(getDefaulFilterState());
    };

    return (
        <div className="teamselection-page">
            {data ? (
                <div>
                    <button
                        style={{ width: 100 }}
                        onClick={() => navigate(`/stage/${raceId}/1`)}>
                        Etappe 1
                    </button>
                    <div style={{ color: "white" }}>
                        Budget Over: {data.budgetOver / 1_000_000}M/
                        {data.budget / 1_000_000}M
                    </div>
                    <div
                        style={{
                            display: "grid",
                            gridTemplateColumns: "1fr 1fr",
                            columnGap: "1rem",
                            marginBottom: "1rem",
                        }}>
                        <FilterElements
                            updateFilter={updateAndFilter}
                            resetFilter={resetFilter}
                            filters={filters}
                            teams={data.allTeams}
                        />
                        <RiderTypeTotals team={data.team} />
                    </div>
                    <div
                        style={{
                            display: "grid",
                            gridTemplateColumns: "1fr 1fr",
                            columnGap: "1rem",
                        }}>
                        <SelectableRidersTable
                            data={filteredRiders}
                            loading={isLoading}
                            addRider={addRider}
                            removeRider={removeRider}
                        />
                        <TeamSelectionTable
                            data={data.team}
                            loading={isLoading}
                        />
                    </div>
                </div>
            ) : (
                <></>
            )}
        </div>
    );
};

export default TeamSelection;

function filterRiders(
    filters: Filters,
    riders: SelectableRider[]
): SelectableRider[] {
    for (const [filterType, value] of Object.entries(filters)) {
        if (!value) {
            continue;
        }
        switch (filterType) {
            case "name":
                riders = riders.filter(({ details }) =>
                    (details.rider.firstname + details.rider.lastname)
                        .toLowerCase()
                        .includes(value.toLowerCase())
                );
                break;
            case "minPrice":
                riders = riders.filter(({ details }) => details.price >= value);
                break;
            case "maxPrice":
                riders = riders.filter(({ details }) => details.price <= value);
                break;
            case "team":
                riders = riders.filter(({ details }) =>
                    details.team.toLowerCase().includes(value.toLowerCase())
                );
                break;
            case "skill":
                riders = riders.filter(({ details }) =>
                    SkillFilter(details, value)
                );
                riders.sort((A, B) => SkillSort(A, B, value));
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
