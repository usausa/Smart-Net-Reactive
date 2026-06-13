namespace Smart.ComponentModel;

using System.ComponentModel;

public sealed class NotifyPropertyChangedExtensionsTest
{
    // A test implementation of INotifyPropertyChanged that tracks add/remove handler calls
    private sealed class TrackingNotifyPropertyChanged : INotifyPropertyChanged
    {
        public int AddCount { get; private set; }

        public int RemoveCount { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                AddCount++;
                InnerPropertyChanged += value;
            }
            remove
            {
                RemoveCount++;
                InnerPropertyChanged -= value;
            }
        }

        private event PropertyChangedEventHandler? InnerPropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            InnerPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Fact]
    public void PropertyChangedAsObservableReceivesNotifications()
    {
        // Arrange
        var source = new TrackingNotifyPropertyChanged();
        var results = new List<PropertyChangedEventArgs>();
        using var subscription = source.PropertyChangedAsObservable().Subscribe(results.Add);

        // Act
        source.RaisePropertyChanged("Foo");
        source.RaisePropertyChanged("Bar");

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("Foo", results[0].PropertyName);
        Assert.Equal("Bar", results[1].PropertyName);
    }

    [Fact]
    public void DisposalRemovesEventHandler()
    {
        // Arrange
        var source = new TrackingNotifyPropertyChanged();

        // Act
        var subscription = source.PropertyChangedAsObservable().Subscribe(_ => { });

        // Assert
        // After subscribing, the handler should have been added
        Assert.Equal(1, source.AddCount);
        Assert.Equal(0, source.RemoveCount);

        // Act
        subscription.Dispose();

        // Assert
        // After disposal, the handler should have been removed
        Assert.Equal(1, source.AddCount);
        Assert.Equal(1, source.RemoveCount);
    }

    [Fact]
    public void NoMoreNotificationsAfterDisposal()
    {
        // Arrange
        var source = new TrackingNotifyPropertyChanged();
        var results = new List<PropertyChangedEventArgs>();
        var subscription = source.PropertyChangedAsObservable().Subscribe(results.Add);

        // Act
        source.RaisePropertyChanged("Before");
        subscription.Dispose();
        source.RaisePropertyChanged("After");

        // Assert
        Assert.Single(results);
        Assert.Equal("Before", results[0].PropertyName);
    }
}
