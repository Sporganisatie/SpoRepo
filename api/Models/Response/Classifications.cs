namespace SpoRE.Models.Response;

public record Classifications(IEnumerable<ClassificationRow> Gc, IEnumerable<ClassificationRow> Points, IEnumerable<ClassificationRow> Kom, IEnumerable<ClassificationRow> Youth)
{
    public static Classifications Empty
        => new([], [], [], []);

    public IEnumerable<ClassificationRow> Stage { get; set; }
};