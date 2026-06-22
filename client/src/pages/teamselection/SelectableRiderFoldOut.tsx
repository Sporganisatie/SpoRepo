import type { SelectableRider } from "./Models/SelectableRider";
import StarRating from "../../components/shared/StarRating";

type SkillKey = "gc" | "sprint" | "climb" | "tt" | "punch";

const SKILLS: { key: SkillKey; label: string }[] = [
  { key: "gc", label: "Klassement" },
  { key: "climb", label: "Klimmen" },
  { key: "sprint", label: "Sprint" },
  { key: "tt", label: "Tijdrijden" },
  { key: "punch", label: "Punch" },
];

const SelectableRiderFoldout = ({ data }: { data: SelectableRider }) => (
  <div className="rider-skills">
    {SKILLS.map(({ key, label }) => (
      <div key={key} className={`stat-tile ${key}`}>
        <div className="stat-tile-label">{label}</div>
        <StarRating score={data.details[key]} />
      </div>
    ))}
  </div>
);

export default SelectableRiderFoldout;
