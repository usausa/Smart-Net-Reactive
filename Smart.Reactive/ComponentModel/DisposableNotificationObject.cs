namespace Smart.ComponentModel;

using System.Reactive.Disposables;

public abstract class DisposableNotificationObject : NotificationObject, IDisposable
{
    protected CompositeDisposable Disposables { get; } = [];

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
