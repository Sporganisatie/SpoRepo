import ArrowSelect from "../../components/ArrowSelect";
import type { SelectOption } from "../../components/Select";
import Select from "../../components/Select";
import { BUDGET_CAP_FREE, BUDGET_CAP_PAID } from "../../lib/constants";

export interface Filters {
  name: string;
  minPrice: number;
  maxPrice: number;
  team: string;
  skill: string;
}

const priceOptions: SelectOption<number>[] = [
  { displayValue: "500.000", value: BUDGET_CAP_FREE },
  { displayValue: "750.000", value: 750000 },
  { displayValue: "1.000.000", value: 1000000 },
  { displayValue: "1.500.000", value: 1500000 },
  { displayValue: "2.000.000", value: 2000000 },
  { displayValue: "2.500.000", value: 2500000 },
  { displayValue: "3.000.000", value: 3000000 },
  { displayValue: "3.500.000", value: 3500000 },
  { displayValue: "4.000.000", value: 4000000 },
  { displayValue: "4.500.000", value: 4500000 },
  { displayValue: "5.000.000", value: 5000000 },
  { displayValue: "5.500.000", value: 5500000 },
  { displayValue: "6.000.000", value: 6000000 },
  { displayValue: "7.000.000", value: 7000000 },
  { displayValue: "8.000.000", value: BUDGET_CAP_PAID },
];

const skillOptions: SelectOption<string>[] = [
  { displayValue: "Alle skills", value: "" },
  { displayValue: "Klassement", value: "gc" },
  { displayValue: "Sprint", value: "sprint" },
  { displayValue: "Klimmen", value: "climb" },
  { displayValue: "Tijdrijden", value: "tt" },
  { displayValue: "Punch", value: "punch" },
];

export interface FiltersProps {
  updateFilter: (part: Partial<Filters>) => void;
  resetFilter: () => void;
  filters: Filters;
  teams: string[];
}

function FilterElements(props: FiltersProps) {
  const teamOptions = [{ displayValue: "Alle teams", value: "" }]
    .concat(props.teams.map((team) => ({ displayValue: team, value: team })))
    .sort();
  return (
    <div className="ts-filters">
      <div className="ts-filter-group">
        <label className="ts-filter-label" htmlFor="filter-name">
          Naam
        </label>
        <div className="ts-filter-controls">
          <input
            id="filter-name"
            type="text"
            value={props.filters.name}
            placeholder="Zoek op naam"
            onChange={(e) => props.updateFilter({ name: e.target.value.trim() })}
          />
        </div>
      </div>
      <div className="ts-filter-group">
        <span className="ts-filter-label">Prijs</span>
        <div className="ts-filter-controls">
          <Select<number>
            value={props.filters.minPrice}
            options={priceOptions}
            onChange={(selectedOption: number) => {
              props.updateFilter({ minPrice: selectedOption });
            }}
          />
          <span className="ts-filter-range-sep">–</span>
          <Select<number>
            value={props.filters.maxPrice}
            options={priceOptions}
            onChange={(selectedOption: number) => {
              props.updateFilter({ maxPrice: selectedOption });
            }}
          />
        </div>
      </div>
      <div className="ts-filter-group">
        <span className="ts-filter-label">Skill</span>
        <div className="ts-filter-controls">
          <Select
            value={props.filters.skill}
            options={skillOptions}
            onChange={(selectedValue) => {
              props.updateFilter({ skill: selectedValue });
            }}
          />
        </div>
      </div>
      <div className="ts-filter-group">
        <span className="ts-filter-label">Team</span>
        <div className="ts-filter-controls">
          <ArrowSelect
            value={props.filters.team}
            allowLooping={true}
            options={teamOptions}
            onChange={(selectedValue) => {
              props.updateFilter({ team: selectedValue });
            }}
          />
        </div>
      </div>
      <div className="ts-filter-spacer" />
      <button className="ts-filter-reset" onClick={() => props.resetFilter()}>
        Reset
      </button>
    </div>
  );
}

export default FilterElements;
