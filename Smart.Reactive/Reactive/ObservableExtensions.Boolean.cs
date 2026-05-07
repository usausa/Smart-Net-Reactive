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
        var result = source;
        foreach (var other in others)
        {
            result = result.And(other);
        }

        return result;
    }

    public static IObservable<bool> Or(this IObservable<bool> source, IObservable<bool> other) =>
        source.CombineLatest(other, static (x, y) => x || y);

    public static IObservable<bool> Or(this IObservable<bool> source, params IObservable<bool>[] others)
    {
        var result = source;
        foreach (var other in others)
        {
            result = result.Or(other);
        }

        return result;
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
