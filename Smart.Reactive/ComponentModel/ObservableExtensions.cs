namespace Smart.ComponentModel
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <param name="holder"></param>
        /// <returns></returns>
        public static IDisposable SubscribeValue<T>(
            this IObservable<T> observable,
            IValueHolder<T> holder)
        {
            return observable.Subscribe(x => holder.Value = x);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="observable"></param>
        /// <param name="selector"></param>
        /// <param name="holder"></param>
        /// <returns></returns>
        public static IDisposable SubscribeValue<TSource, TResult>(
            this IObservable<TSource> observable,
            Func<TSource, TResult> selector,
            IValueHolder<TResult> holder)
        {
            return observable.Subscribe(x => holder.Value = selector(x));
        }
    }
}
