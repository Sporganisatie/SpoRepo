import ArrowSelect from '../../components/ArrowSelect';
import Select, { SelectOption } from '../../components/Select';

export interface Filters {
    name: string,
    minPrice: number,
    maxPrice: number,
    team: string,
    skill: string,
};

const priceOptions: SelectOption<number>[] = [
    { displayValue: '500.000', value: 500000 },
    { displayValue: '750.000', value: 750000 },
    { displayValue: '1.000.000', value: 1000000 },
    { displayValue: '1.500.000', value: 1500000 },
    { displayValue: '2.000.000', value: 2000000 },
    { displayValue: '2.500.000', value: 2500000 },
    { displayValue: '3.000.000', value: 3000000 },
    { displayValue: '3.500.000', value: 3500000 },
    { displayValue: '4.000.000', value: 4000000 },
    { displayValue: '4.500.000', value: 4500000 },
    { displayValue: '5.000.000', value: 5000000 },
    { displayValue: '5.500.000', value: 5500000 },
    { displayValue: '6.000.000', value: 6000000 },
    { displayValue: '7.000.000', value: 7000000 },
    { displayValue: '8.000.000', value: 8000000 },
];

const skillOptions: SelectOption<string>[] = [
    { displayValue: '', value: '' },
    { displayValue: 'Klassement', value: 'gc' },
    { displayValue: 'Sprint', value: 'sprint' },
    { displayValue: 'Klimmen', value: 'climb' },
    { displayValue: 'Tijdrijden', value: 'tt' },
    { displayValue: 'Punch', value: 'punch' },
];

export interface FiltersProps {
    updateFilter: (part: Partial<Filters>) => void;
    resetFilter: () => void;
    filters: Filters,
    teams: string[]
}

function FilterElements(props: FiltersProps) {
    var teamOptions = [{ displayValue: "Alle teams", value: "" }].concat(props.teams.map(team => ({ displayValue: team, value: team }))).sort()
    return (
        <div style={{ display: "flex", flexDirection: "row", gap: "0.5rem", margin: "1rem" }}>
            <div style={{ display: 'flex', alignItems: 'center' }}>
                <input
                    type="text"
                    value={props.filters.name}
                    placeholder="naam"
                    onChange={(e) => props.updateFilter({ name: e.target.value.trim() })}
                />
            </div>
            <Select<number>
                value={props.filters.minPrice}
                options={priceOptions}
                onChange={(selectedOption: number) => {
                    props.updateFilter({ minPrice: selectedOption });
                }}
            />
            <Select<number>
                value={props.filters.maxPrice}
                options={priceOptions}
                onChange={(selectedOption: number) => {
                    props.updateFilter({ maxPrice: selectedOption });
                }}
            />
            <Select
                value={props.filters.skill}
                options={skillOptions}
                onChange={(selectedValue) => {
                    props.updateFilter({ skill: selectedValue });
                }} />
            <ArrowSelect
                value={props.filters.team}
                allowLooping={true}
                options={teamOptions}
                onChange={(selectedValue) => {
                    props.updateFilter({ team: selectedValue });
                }} />
            <div style={{ display: 'flex', alignItems: 'center' }}>
                <button onClick={() => props.resetFilter()}>
                    Reset
                </button>
            </div>
        </div>
    );
}

export default FilterElements;