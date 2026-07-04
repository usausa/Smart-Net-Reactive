namespace Smart.Reactive;

using System.Reactive.Linq;

public static class ObservableBooleanExtensions
{
    public static IObservable<bool> Not(this IObservable<bool> source) =>
        source.Select(static x => !x);

    public static IObservable<bool> And(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => x && y);

    public static IObservable<bool> And(this IObservable<bool> source, params IObservable<bool>[] others)
    {
        if (others.Length == 0)
        {
            return source;
        }

        // Few inputs: chained binary CombineLatest has less fixed overhead
        // Many inputs: a single collection CombineLatest avoids deep operator nesting
        if (others.Length <= 3)
        {
            var result = source;
            foreach (var other in others)
            {
                result = result.And(other);
            }

            return result;
        }

        var sources = new IObservable<bool>[others.Length + 1];
        sources[0] = source;
        Array.Copy(others, 0, sources, 1, others.Length);
        return sources.CombineLatest(static values =>
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (!values[i])
                {
                    return false;
                }
            }

            return true;
        });
    }

    public static IObservable<bool> Or(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => x || y);

    public static IObservable<bool> Or(this IObservable<bool> source, params IObservable<bool>[] others)
    {
        if (others.Length == 0)
        {
            return source;
        }

        // Few inputs: chained binary CombineLatest has less fixed overhead
        // Many inputs: a single collection CombineLatest avoids deep operator nesting
        if (others.Length <= 3)
        {
            var result = source;
            foreach (var other in others)
            {
                result = result.Or(other);
            }

            return result;
        }

        var sources = new IObservable<bool>[others.Length + 1];
        sources[0] = source;
        Array.Copy(others, 0, sources, 1, others.Length);
        return sources.CombineLatest(static values =>
        {
            for (var i = 0; i < values.Count; i++)
            {
                if (values[i])
                {
                    return true;
                }
            }

            return false;
        });
    }

    public static IObservable<bool> Xor(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => x ^ y);

    public static IObservable<bool> Nand(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => !(x && y));

    public static IObservable<bool> Nor(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => !(x || y));

    public static IObservable<bool> Xnor(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => x == y);
}
