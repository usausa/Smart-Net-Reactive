namespace Smart.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    /// <summary>
    ///
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<PropertyChangedEventArgs> AsObservable(
            this INotifyPropertyChanged source)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (s, e) => h(e),
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IObservable<T> AsObservable<T>(
            this T source,
            string propertyName)
            where T : INotifyPropertyChanged
        {
            return source
                .AsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Select(x => source);
        }
    }
}
