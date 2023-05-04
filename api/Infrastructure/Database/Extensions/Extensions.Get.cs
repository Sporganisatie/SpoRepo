using System.Linq.Expressions;

namespace SpoRE.Infrastructure.Database;

public static class Extensions
{
    public static Task<Result<TSource>> SingleResult<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        try
        {
            return Result.For(source.Single(predicate)).AsTask();
        }
        catch (System.Exception ex)
        {
            return Result.WithMessages<TSource>(ValidationMessage.Error(ex.Message)).AsTask();
        }
    }

    public static Task<Result<List<TSource>>> ListResult<TSource>(this IQueryable<TSource> source)
    {
        try
        {
            return Result.For(source.ToList()).AsTask();
        }
        catch (System.Exception ex)
        {
            return Result.WithMessages<List<TSource>>(ValidationMessage.Error(ex.Message)).AsTask();
        }
    }
}