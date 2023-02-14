namespace SpoRE.Infrastructure.Database;

public partial class ResultsPoint
{
    public int StageId { get; set; }

    public int RiderParticipationId { get; set; }

    public int? Stagepos { get; set; }

    public int? Gcpos { get; set; }

    public int? Pointspos { get; set; }

    public int? Kompos { get; set; }

    public int? Yocpos { get; set; }

    public int? Stagescore { get; set; }

    public int? Gcscore { get; set; }

    public int? Pointsscore { get; set; }

    public int? Komscore { get; set; }

    public int? Yocscore { get; set; }

    public int? Teamscore { get; set; }

    public int? Totalscore { get; set; }

    public string Stageresult { get; set; }

    public string Gcresult { get; set; }

    public string Pointsresult { get; set; }

    public string Komresult { get; set; }

    public string Yocresult { get; set; }

    public string Gcprev { get; set; }

    public string Gcchange { get; set; }

    public string Pointsprev { get; set; }

    public string Pointschange { get; set; }

    public string Komprev { get; set; }

    public string Komchange { get; set; }

    public string Yocprev { get; set; }

    public string Yocchange { get; set; }

    public virtual RiderParticipation RiderParticipation { get; set; }

    public virtual Stage Stage { get; set; }
}
