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
        public static IDisposable SubscribeValue<T>(this IObservable<T> observable, IValueHolder<T> holder)
        {
            return observable.Subscribe(_ => holder.Value = _);
        }
    }
}
