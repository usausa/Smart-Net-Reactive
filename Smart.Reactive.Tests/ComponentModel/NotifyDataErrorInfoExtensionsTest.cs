namespace Smart.ComponentModel;

using System.Collections;
using System.ComponentModel;

public sealed class NotifyDataErrorInfoExtensionsTest
{
    // A test implementation of INotifyDataErrorInfo that tracks add/remove handler calls
    private sealed class TrackingNotifyDataErrorInfo : INotifyDataErrorInfo
    {
        public int AddCount { get; private set; }

        public int RemoveCount { get; private set; }

        public bool HasErrors => false;

        public IEnumerable GetErrors(string? propertyName) => Array.Empty<object>();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged
        {
            add
            {
                AddCount++;
                InnerErrorsChanged += value;
            }
            remove
            {
                RemoveCount++;
                InnerErrorsChanged -= value;
            }
        }

        private event EventHandler<DataErrorsChangedEventArgs>? InnerErrorsChanged;

        public void RaiseErrorsChanged(string propertyName)
        {
            InnerErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    [Fact]
    public void ErrorsChangedAsObservableReceivesNotifications()
    {
        // Arrange
        var source = new TrackingNotifyDataErrorInfo();
        var results = new List<DataErrorsChangedEventArgs>();
        using var subscription = source.ErrorsChangedAsObservable().Subscribe(results.Add);

        // Act
        source.RaiseErrorsChanged("Foo");
        source.RaiseErrorsChanged("Bar");

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("Foo", results[0].PropertyName);
        Assert.Equal("Bar", results[1].PropertyName);
    }

    [Fact]
    public void DisposalRemovesEventHandler()
    {
        // Arrange
        var source = new TrackingNotifyDataErrorInfo();

        // Act
        var subscription = source.ErrorsChangedAsObservable().Subscribe(_ => { });

        // Assert
        Assert.Equal(1, source.AddCount);
        Assert.Equal(0, source.RemoveCount);

        // Act
        subscription.Dispose();

        // Assert
        Assert.Equal(1, source.AddCount);
        Assert.Equal(1, source.RemoveCount);
    }

    [Fact]
    public void NoMoreNotificationsAfterDisposal()
    {
        // Arrange
        var source = new TrackingNotifyDataErrorInfo();
        var results = new List<DataErrorsChangedEventArgs>();
        var subscription = source.ErrorsChangedAsObservable().Subscribe(results.Add);

        // Act
        source.RaiseErrorsChanged("Before");
        subscription.Dispose();
        source.RaiseErrorsChanged("After");

        // Assert
        Assert.Single(results);
        Assert.Equal("Before", results[0].PropertyName);
    }

    [Fact]
    public void ErrorsChangedAsObservableWithPropertyNameFilters()
    {
        // Arrange
        var source = new TrackingNotifyDataErrorInfo();
        var results = new List<TrackingNotifyDataErrorInfo>();
        using var subscription = source
            .ErrorsChangedAsObservable("Foo")
            .Subscribe(results.Add);

        // Act
        source.RaiseErrorsChanged("Foo");
        source.RaiseErrorsChanged("Bar");
        source.RaiseErrorsChanged("Foo");

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Same(source, r));
    }
}
