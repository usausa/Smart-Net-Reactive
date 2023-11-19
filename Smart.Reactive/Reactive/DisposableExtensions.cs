namespace Smart.Reactive;

using System.Reactive.Disposables;

public static class DisposableExtensions
{
#pragma warning disable CA1062
    public static T AddTo<T>(this T disposable, CompositeDisposable disposables)
        where T : IDisposable
    {
        disposables.Add(disposable);
        return disposable;
    }
#pragma warning restore CA1062
}
