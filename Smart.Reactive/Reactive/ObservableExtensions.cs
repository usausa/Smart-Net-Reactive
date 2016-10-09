namespace Smart.Reactive
{
    using System;
    using System.Reactive.Linq;

    /// <summary>
    ///
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        public static IObservable<TResult> Pairwise<T, TResult>(this IObservable<T> source, Func<T, T, TResult> selector)
        {
            return Observable.Create<TResult>(observer =>
            {
                var prev = default(T);
                var isFirst = true;

                return source.Subscribe(x =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        prev = x;
                    }
                    else
                    {
                        TResult value;
                        try
                        {
                            value = selector(prev, x);
                            prev = x;
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            return;
                        }

                        observer.OnNext(value);
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }
    }
}
