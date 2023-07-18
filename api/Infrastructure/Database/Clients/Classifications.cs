namespace SpoRE.Models.Response;

public record Classifications(IEnumerable<ClassificationRow> Gc, IEnumerable<ClassificationRow> Points, IEnumerable<ClassificationRow> Kom, IEnumerable<ClassificationRow> Youth)
{
    public static Classifications Empty
        => new(new List<ClassificationRow>(), new List<ClassificationRow>(), new List<ClassificationRow>(), new List<ClassificationRow>());

    public IEnumerable<ClassificationRow> Stage { get; set; }
};