import { useRaceContext } from "../../shared/RaceContextProvider";
import DropdownMenu from "./base/DropdownMenu";

const ChartsDropdown = (props: { raceSelected: boolean }) => {
  const race = useRaceContext();

  return (
    <DropdownMenu
      {...props}
      name="Charts"
      alwaysLinks={[
        // { url: "/charts/totalscorespread", title: "Score verdeling Totaal" }
      ]}
      raceOnlyLinks={[
        { url: `/${race}/charts/scoreverloop`, title: "Relatief Scoreverloop" },
        { url: `/${race}/charts/scoreverdeling`, title: "Score Verdeling" }
      ]}
    />
  );
};

export default ChartsDropdown;
