using SpoRE.Infrastructure.Database;

namespace SpoRE.Models.Response;

public record ClassificationRow
{
    public Rider Rider { get; internal set; }
    public string Team { get; internal set; }
    public BaseResult Result { get; internal set; }
    public StageSelectedEnum Selected { get; internal set; }
}