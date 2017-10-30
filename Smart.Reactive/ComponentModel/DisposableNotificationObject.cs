namespace Smart.ComponentModel
{
    using System;
    using System.Reactive.Disposables;

    /// <summary>
    ///
    /// </summary>
    public abstract class DisposableNotificationObject : NotificationObject, IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        /// <summary>
        ///
        /// </summary>
        ~DisposableNotificationObject()
        {
            Dispose(false);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposables.Dispose();
            }
        }
    }
}
