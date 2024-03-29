import DropdownMenu from "./base/DropdownMenu";

const StatistiekenDropdown = (props: { raceSelected: boolean }) => {
    return (
        <DropdownMenu
            {...props}
            name="Statistieken"
            alwaysLinks={[ // Deze misschien in een losse dropdown of zo?
                { url: "/raceUitslagen", title: "Uitslagen per race" }, // TODO dynamic race id
            ]}
            raceOnlyLinks={[
                { url: "/teamcomparison/29", title: "Team overzichten" }, // TODO dynamic race id
                { url: "/etappeUitslagen/29", title: "Uitslagen per etappe" }, // TODO dynamic race id
                { url: "/allRiders/29", title: "Alle Renners" }, // TODO dynamic race id
                { url: "/missedpoints/29", title: "Gemiste punten" }, // TODO dynamic race id
                { url: "/uitvallers/29", title: "Uitvallers" }, // TODO dynamic race id
                { url: "/klassementen/29", title: "Klassementen" }, // TODO dynamic race id
                // { url: "/statistics/klassementen", title: "Klassementen" },
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/teamcomparisons", title: "Selectie vergelijking" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;