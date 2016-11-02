namespace Smart.ComponentModel
{
    using System;
    using System.Reactive.Subjects;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ObservableValue<T> : IValueHolder<T>, IObservable<T>, IDisposable
    {
        private readonly Subject<T> subject = new Subject<T>();

        private T lastValue;

        /// <summary>
        ///
        /// </summary>
        public T Value
        {
            get
            {
                return lastValue;
            }
            set
            {
                subject.OnNext(value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ObservableValue()
        {
            subject.Subscribe(x => lastValue = x);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (subject.IsDisposed)
            {
                return;
            }

            subject.OnCompleted();
            subject.Dispose();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
