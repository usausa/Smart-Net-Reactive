namespace Smart.ComponentModel
{
    using System;
    using System.Reactive.Disposables;

    public abstract class DisposableNotificationObject : NotificationObject, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        ~DisposableNotificationObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposables.Dispose();
            }
        }
    }
}
