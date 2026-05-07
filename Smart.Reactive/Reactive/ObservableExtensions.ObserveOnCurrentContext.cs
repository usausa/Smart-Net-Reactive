#pragma warning disable SA1649 // File name should match first type name
namespace Smart.Reactive;

// Use case examples:
//
// // UI スレッドで購読する (SynchronizationContext が設定されている場面で使用)
// someObservable
//     .ObserveOnCurrentContext()
//     .Subscribe(x => label.Text = x.ToString());
using System.Reactive.Linq;

public static class ObservableObserveOnCurrentContextExtensions
{
    public static IObservable<TSource> ObserveOnCurrentContext<TSource>(this IObservable<TSource> source)
    {
        var context = SynchronizationContext.Current ?? throw new InvalidOperationException("Current synchronization context is null.");
        return source.ObserveOn(context);
    }
}
