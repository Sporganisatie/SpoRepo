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
                { url: `/teamcomparison/${race}`, title: "Team overzichten" },
                { url: `/etappeUitslagen/${race}`, title: "Uitslagen per etappe" },
                { url: `/allRiders/${race}`, title: "Alle Renners" },
                { url: `/missedpoints/${race}`, title: "Gemiste punten" },
                { url: `/uitvallers/${race}`, title: "Uitvallers" },
                { url: `/klassementen/${race}`, title: "Klassementen" },
                { url: `/teamoverlap/${race}`, title: "Team Overlap" },
                { url: `/uniekheid/${race}`, title: "Uniekheid" }
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;