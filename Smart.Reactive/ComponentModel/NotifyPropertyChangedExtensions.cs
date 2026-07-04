namespace Smart.ComponentModel;

using System.ComponentModel;
using System.Reactive.Linq;

public static class NotifyPropertyChangedExtensions
{
    public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable(
        this INotifyPropertyChanged source)
    {
        return new PropertyChangedObservable(source);
    }

    public static IObservable<T> AsObservable<T>(
        this T source,
        string propertyName)
        where T : INotifyPropertyChanged
    {
        return new PropertyChangedFilterObservable<T>(source, propertyName);
    }

    public static IObservable<T> AsObservable<T>(
        this T source)
        where T : INotifyPropertyChanged
    {
        return source
            .PropertyChangedAsObservable()
            .Select(_ => source);
    }

    public static IObservable<T> AsValueObservable<T>(
        this NotificationValue<T> source)
    {
        return source
            .PropertyChangedAsObservable()
            .Where(x => string.IsNullOrEmpty(x.PropertyName) || (x.PropertyName == nameof(NotificationValue<>.Value)))
            .Select(_ => source.Value);
    }

    private sealed class PropertyChangedObservable : IObservable<PropertyChangedEventArgs>
    {
        private readonly INotifyPropertyChanged source;

        public PropertyChangedObservable(INotifyPropertyChanged source)
        {
            this.source = source;
        }

        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer) => new Subscription(source, observer);

        private sealed class Subscription : IDisposable
        {
            private readonly INotifyPropertyChanged source;

            private IObserver<PropertyChangedEventArgs>? observer;

            public Subscription(INotifyPropertyChanged source, IObserver<PropertyChangedEventArgs> observer)
            {
                this.source = source;
                this.observer = observer;
                source.PropertyChanged += OnPropertyChanged;
            }

            private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => observer?.OnNext(e);

            public void Dispose()
            {
                if (observer is not null)
                {
                    source.PropertyChanged -= OnPropertyChanged;
                    observer = null;
                }
            }
        }
    }

    private sealed class PropertyChangedFilterObservable<T> : IObservable<T>
        where T : INotifyPropertyChanged
    {
        private readonly T source;

        private readonly string propertyName;

        public PropertyChangedFilterObservable(T source, string propertyName)
        {
            this.source = source;
            this.propertyName = propertyName;
        }

        public IDisposable Subscribe(IObserver<T> observer) => new Subscription(source, propertyName, observer);

        private sealed class Subscription : IDisposable
        {
            private readonly T source;

            private readonly string propertyName;

            private IObserver<T>? observer;

            public Subscription(T source, string propertyName, IObserver<T> observer)
            {
                this.source = source;
                this.propertyName = propertyName;
                this.observer = observer;
                source.PropertyChanged += OnPropertyChanged;
            }

            private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == propertyName))
                {
                    observer?.OnNext(source);
                }
            }

            public void Dispose()
            {
                if (observer is not null)
                {
                    source.PropertyChanged -= OnPropertyChanged;
                    observer = null;
                }
            }
        }
    }
}
