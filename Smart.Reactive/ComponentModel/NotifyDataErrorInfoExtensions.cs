namespace Smart.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    /// <summary>
    ///
    /// </summary>
    public static class NotifyDataErrorInfoExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable(
            this INotifyDataErrorInfo source)
        {
            return Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.ErrorsChanged += h,
                h => source.ErrorsChanged -= h);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IObservable<T> ErrorsChangedAsObservable<T>(
            this T source,
            string propertyName)
            where T : INotifyDataErrorInfo
        {
            return source
                .ErrorsChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Select(x => source);
        }
    }
}
