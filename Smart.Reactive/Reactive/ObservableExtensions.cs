namespace Smart.Reactive.Reactive
{
    using System;
    using Smart.ComponentModel;

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
        public static IDisposable Bind<T>(this IObservable<T> observable, IValueHolder<T> holder)
        {
            return observable.Subscribe(_ => holder.Value = _);
        }
    }
}
