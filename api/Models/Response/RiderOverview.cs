namespace SpoRE.Models.Response;

public record RiderRaceOverviewRow(
    int RaceId,
    string RaceName,
    int Year,
    bool Active,
    bool Dnf,
    int Price,
    bool TooExpensive,
    int TotalPoints,
    int TotalParticipants,
    int SelectedCount,
    int? GcPosition,
    int? PointsPosition,
    int? KomPosition,
    int? YouthPosition);

public record RiderSelectionHistoryRow(
    string Username,
    int SelectedCount,
    int TotalCount,
    IEnumerable<string> Races);

public record RiderOverview(
    int RiderId,
    string Firstname,
    string Lastname,
    IEnumerable<RiderRaceOverviewRow> Races,
    IEnumerable<RiderSelectionHistoryRow> SelectionHistory);

