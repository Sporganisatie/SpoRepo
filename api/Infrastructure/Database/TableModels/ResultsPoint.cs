﻿namespace SpoRE.Infrastructure.Database;

public class BaseResult
{
    public int? Position { get; set; }
    public int? Score { get; set; }
    public string Result { get; set; }
    public string Change { get; set; }
}

public class ResultsPoint
{
    public int StageId { get; set; }
    public int RiderParticipationId { get; set; }
    public int? StagePos { get; set; }
    public int? StageScore { get; set; }
    public string StageResult { get; set; }
    public BaseResult Gc { get; set; }
    public BaseResult Points { get; set; }
    public BaseResult Kom { get; set; }
    public BaseResult Youth { get; set; }
    public int? Teamscore { get; set; }
    public int? Totalscore { get; set; }
    public virtual RiderParticipation RiderParticipation { get; set; }
    public virtual Stage Stage { get; set; }
}
