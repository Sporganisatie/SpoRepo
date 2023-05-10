import DropdownMenu from "./base/DropdownMenu";

const StatistiekenDropdown = (props: { raceSelected: boolean }) => {
    return (
        <DropdownMenu
            {...props}
            name="Statistieken"
            alwaysLinks={[
                // { url: "/statistics/rondewinsten", title: "Uitslagen per ronde" }
            ]}
            raceOnlyLinks={[
                { url: "/teamcomparison/27/1", title: "Team overzichten" }, // TODO dynamic race id
                // { url: "/statistics/etappewinsten", title: "Uitslagen per etappe" },
                // { url: "/statistics/allriders", title: "Alle renners" },
                // { url: "/statistics/klassementen", title: "Klassementen" },
                // { url: "/statistics/missedpointsall", title: "Gemiste punten iedereen" },
                // { url: "/statistics/missedPointsPerRider", title: "Gemiste punten per Renner" },
                // { url: "/statistics/teamcomparisons", title: "Selectie vergelijking" },
                // { url: "/statistics/overigestats", title: "Overige Statistieken" }
            ]} />
    )
}

export default StatistiekenDropdown;