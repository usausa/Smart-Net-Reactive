namespace Smart.ComponentModel;

public class ObservableValue<T> : NotificationValue<T>, IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        return this.AsValueObservable().Subscribe(observer);
    }
}
