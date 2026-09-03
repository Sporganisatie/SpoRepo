namespace SpoRE.Models.Response;

public record RiderStageScoreRow(
    int? Stagenr,
    string Label,
    int? StageScore,
    int GcScore,
    int PointsScore,
    int KomScore,
    int YouthScore,
    int TeamScore,
    int TotalScore);

public record ClassificationCell(int? Position, string Result);

public record RiderClassificationRow(
    int? Stagenr,
    string Label,
    int? StagePos,
    string StageResult,
    ClassificationCell Gc,
    ClassificationCell Points,
    ClassificationCell Kom,
    ClassificationCell Youth);

public record RiderSelectionRow(
    string Username,
    int Points,
    int TimesSelected,
    int TimesKopman,
    bool IsLoggedInUser);

public record RiderRaceDetail(
    string Firstname,
    string Lastname,
    bool TooExpensive,
    bool Active,
    bool Dnf,
    int Price,
    int TotalParticipants,
    IEnumerable<RiderStageScoreRow> Stages,
    IEnumerable<RiderClassificationRow> Classifications,
    IEnumerable<RiderSelectionRow> Selections);

