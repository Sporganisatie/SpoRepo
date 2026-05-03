import type { ExpanderComponentProps } from "react-data-table-component";
import type { SelectableRider } from "./Models/SelectableRider";
import StarRating from "../../components/shared/StarRating";

type SkillKey = "gc" | "sprint" | "climb" | "tt" | "punch";

const SKILLS: { key: SkillKey; label: string }[] = [
  { key: "gc", label: "Klassement" },
  { key: "sprint", label: "Sprint" },
  { key: "climb", label: "Klimmen" },
  { key: "tt", label: "Tijdrijden" },
  { key: "punch", label: "Punch" },
];

const SelectableRiderFoldout: React.FC<ExpanderComponentProps<SelectableRider>> = ({ data }) => (
  <div className="rider-skills">
    {SKILLS.map(({ key, label }) => (
      <div key={key} className={`rider-skill-tile ${key}`}>
        <div className="rider-skill-tile__label">{label}</div>
        <StarRating score={data.details[key]} />
      </div>
    ))}
  </div>
);

export default SelectableRiderFoldout;
