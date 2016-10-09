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
        public static IObservable<DataErrorsChangedEventArgs> AsObservable(this INotifyDataErrorInfo source)
        {
            return Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.ErrorsChanged += h,
                h => source.ErrorsChanged -= h);
        }
    }
}
