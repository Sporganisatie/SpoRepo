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
                { url: "/teamcomparison/30", title: "Team overzichten" }, // TODO dynamic race id
                { url: "/etappeUitslagen/30", title: "Uitslagen per etappe" }, // TODO dynamic race id
                { url: "/allRiders/30", title: "Alle Renners" }, // TODO dynamic race id
                { url: "/missedpoints/30", title: "Gemiste punten" }, // TODO dynamic race id
                { url: "/uitvallers/30", title: "Uitvallers" }, // TODO dynamic race id
                { url: "/klassementen/30", title: "Klassementen" }, // TODO dynamic race id
                { url: "/teamoverlap/30", title: "Team Overlap" }, // TODO dynamic race id
                // { url: "/statistics/klassementen", title: "Klassementen" },
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;