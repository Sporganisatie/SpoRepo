namespace SpoRE.Infrastructure.Scrape;

public partial class Scrape
{
    private static string TeamNameAlias(string teamName)
    {
        Console.WriteLine(teamName);
        return teamName switch
        {
            "Astana Qazaqstan Team" => "Astana",
            "Bahrain - Victorious" => "Bahrain",
            "Decathlon AG2R La Mondiale Team" => "Decathlon AG2R",
            "EF Education - EasyPost" => "EF Education",
            "INEOS Grenadiers" => "INEOS",
            "Team Visma | Lease a Bike" => "Visma LAB",
            "Movistar Team" => "Movistar",
            "Team dsm-firmenich PostNL" => "dsm-firmenich PostNL",
            "Team Jayco AlUla" => "Jayco AlUla",
            "UAE Team Emirates" => "UAE",
            _ => teamName
        };
    }
}
