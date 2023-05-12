using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record ClassificationRow
{
    public Rider Rider { get; internal set; }
    public int? Position { get; internal set; } // TODO niet nullable
    public string Result { get; internal set; }
    public StageSelectedEnum Selected { get; internal set; }
}