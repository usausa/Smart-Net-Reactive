namespace Smart.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable(
            this INotifyPropertyChanged source)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (s, e) => h(e),
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h);
        }

        public static IObservable<T> AsObservable<T>(
            this T source,
            string propertyName)
            where T : INotifyPropertyChanged
        {
            return source
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == propertyName)
                .Select(x => source);
        }

        public static IObservable<T> AsObservable<T>(
            this T source)
            where T : INotifyPropertyChanged
        {
            return source
                .PropertyChangedAsObservable()
                .Select(x => source);
        }

        public static IObservable<T> AsValueObservable<T>(
            this NotificationValue<T> source)
        {
            return source
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(NotificationValue<T>.Value))
                .Select(x => source.Value);
        }
    }
}
