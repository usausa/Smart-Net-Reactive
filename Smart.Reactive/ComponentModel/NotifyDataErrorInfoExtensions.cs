namespace Smart.ComponentModel;

using System.ComponentModel;

public static class NotifyDataErrorInfoExtensions
{
    public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable(
        this INotifyDataErrorInfo source)
    {
        return new DataErrorsChangedObservable(source);
    }

    public static IObservable<T> ErrorsChangedAsObservable<T>(
        this T source,
        string propertyName)
        where T : INotifyDataErrorInfo
    {
        return new DataErrorsChangedFilterObservable<T>(source, propertyName);
    }

    private sealed class DataErrorsChangedObservable : IObservable<DataErrorsChangedEventArgs>
    {
        private readonly INotifyDataErrorInfo source;

        public DataErrorsChangedObservable(INotifyDataErrorInfo source)
        {
            this.source = source;
        }

        public IDisposable Subscribe(IObserver<DataErrorsChangedEventArgs> observer) => new Subscription(source, observer);

        private sealed class Subscription : IDisposable
        {
            private readonly INotifyDataErrorInfo source;

            private IObserver<DataErrorsChangedEventArgs>? observer;

            public Subscription(INotifyDataErrorInfo source, IObserver<DataErrorsChangedEventArgs> observer)
            {
                this.source = source;
                this.observer = observer;
                source.ErrorsChanged += OnErrorsChanged;
            }

            private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e) =>
                observer?.OnNext(e);

            public void Dispose()
            {
                if (observer is not null)
                {
                    source.ErrorsChanged -= OnErrorsChanged;
                    observer = null;
                }
            }
        }
    }

    private sealed class DataErrorsChangedFilterObservable<T> : IObservable<T>
        where T : INotifyDataErrorInfo
    {
        private readonly T source;

        private readonly string propertyName;

        public DataErrorsChangedFilterObservable(T source, string propertyName)
        {
            this.source = source;
            this.propertyName = propertyName;
        }

        public IDisposable Subscribe(IObserver<T> observer) =>
            new Subscription(source, propertyName, observer);

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
                source.ErrorsChanged += OnErrorsChanged;
            }

            private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
            {
                if (e.PropertyName == propertyName)
                {
                    observer?.OnNext(source);
                }
            }

            public void Dispose()
            {
                if (observer is not null)
                {
                    source.ErrorsChanged -= OnErrorsChanged;
                    observer = null;
                }
            }
        }
    }
}
