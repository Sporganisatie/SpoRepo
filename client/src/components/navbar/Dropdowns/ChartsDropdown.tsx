import DropdownMenu from "./base/DropdownMenu";

const ChartsDropdown = (props: { raceSelected: boolean }) => {
  return (
    <DropdownMenu
      {...props}
      name="Charts"
      alwaysLinks={[
        // { url: "/charts/totalscorespread", title: "Score verdeling Totaal" }
      ]}
      raceOnlyLinks={[
        { url: "/charts/scoreverloop/27", title: "Relatief Scoreverloop" }, // TODO dynamic race id
        { url: "/charts/scoreverloop", title: "Relatief Scoreverloop" }, // TODO dynamic race id
        { url: "/charts/positieverloop/27", title: "Positie Scoreverloop" }, // TODO dynamic race id
        { url: "/charts/scoreverdeling/27", title: "Score Verdeling" }, // TODO dynamic race id
        // { url: "/charts/userscores", title: "Relatief Scoreverloop" },
        // { url: "/charts/userrank", title: "Ranking" },
        // { url: "/charts/riderpercentage", title: "Puntenaandeel Renner per Etappe" },
        // { url: "/charts/scorespread", title: "Score verdeling" },
        // { url: "/charts/optimalpoints", title: "Optimale punten verloop" },
        // { url: "/charts/missedpoints", title: "Gemiste punten verloop" },
        // { url: "/charts/missedpointsspread", title: "Gemiste punten verdeling" },
        // { url: "/charts/missedpointsspreadrelatief", title: "Relatieve gemiste punten verdeling" }
      ]}
    />
  );
};

export default ChartsDropdown;
