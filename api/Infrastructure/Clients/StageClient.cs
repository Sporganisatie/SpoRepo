public record TeamResultRow : RiderRow
{
    public TeamResultRow()
    { }
    public int stagepos { get; set; }
    public int stagescore { get; set; }
    public bool isKopman { get; set; }
}

public record RiderRow
{
    public string lastname { get; set; }
    public string firstname { get; set; }
    public string initials { get; set; }
    public string country { get; set; }
    public int rider_id { get; set; }
}