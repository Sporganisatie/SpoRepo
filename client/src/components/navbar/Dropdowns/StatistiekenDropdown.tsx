import { useRaceContext } from "../../shared/RaceContextProvider";
import DropdownMenu from "./base/DropdownMenu";

const StatistiekenDropdown = (props: { raceSelected: boolean }) => {
    const race = useRaceContext();

    return (
        <DropdownMenu
            {...props}
            name="Statistieken"
            alwaysLinks={[ // Deze misschien in een losse dropdown of zo?
                { url: "/raceUitslagen", title: "Uitslagen per race" },
            ]}
            raceOnlyLinks={[
                { url: `/${race}/teamcomparison`, title: "Team overzichten" },
                { url: `/${race}/etappeUitslagen`, title: "Uitslagen per etappe" },
                { url: `/${race}/allRiders`, title: "Alle Renners" },
                { url: `/${race}/missedpoints`, title: "Gemiste punten" },
                { url: `/${race}/uitvallers`, title: "Uitvallers" },
                { url: `/${race}/klassementen`, title: "Klassementen" },
                { url: `/${race}/teamoverlap`, title: "Team Overlap" },
                { url: `/${race}/uniekheid`, title: "Uniekheid" }
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;