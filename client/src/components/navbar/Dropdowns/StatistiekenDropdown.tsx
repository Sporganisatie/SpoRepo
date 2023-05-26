import DropdownMenu from "./base/DropdownMenu";

const StatistiekenDropdown = (props: { raceSelected: boolean }) => {
    return (
        <DropdownMenu
            {...props}
            name="Statistieken"
            alwaysLinks={[
                { url: "/raceUitslagen", title: "Uitslagen per race" }, // TODO dynamic race id
            ]}
            raceOnlyLinks={[
                { url: "/allRiders/27", title: "Alle Renners" }, // TODO dynamic race id
                { url: "/missedpoints/27", title: "Gemiste punten" }, // TODO dynamic race id
                { url: "/uitvallers/27", title: "Uitvallers" }, // TODO dynamic race id
                { url: "/teamcomparison/27", title: "Team overzichten" }, // TODO dynamic race id
                { url: "/etappeUitslagen/27", title: "Uitslagen per etappe" }, // TODO dynamic race id
                // { url: "/statistics/klassementen", title: "Klassementen" },
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/teamcomparisons", title: "Selectie vergelijking" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;