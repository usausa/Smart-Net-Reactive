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

        public void RaisePropertyChanged(string? propertyName)
        {
            InnerPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // A NotificationValue subclass that allows raising PropertyChanged with an arbitrary name for testing.
    private sealed class TestNotificationValue<T> : NotificationValue<T>
    {
        public void Raise(string? propertyName)
        {
            RaisePropertyChanged(propertyName!);
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

    [Fact]
    public void AsObservableWithPropertyNameFiltersMatching()
    {
        // Arrange
        var source = new TrackingNotifyPropertyChanged();
        var results = new List<TrackingNotifyPropertyChanged>();
        using var subscription = source.AsObservable("Foo").Subscribe(results.Add);

        // Act
        source.RaisePropertyChanged("Foo");
        source.RaisePropertyChanged("Bar");
        source.RaisePropertyChanged("Foo");

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Same(source, r));
    }

    [Fact]
    public void AsObservableTreatsNullOrEmptyPropertyNameAsAllChanged()
    {
        // Arrange
        var source = new TrackingNotifyPropertyChanged();
        var results = new List<TrackingNotifyPropertyChanged>();
        using var subscription = source.AsObservable("Foo").Subscribe(results.Add);

        // Act
        source.RaisePropertyChanged(null);          // null => all properties changed
        source.RaisePropertyChanged(string.Empty);  // empty => all properties changed
        source.RaisePropertyChanged("Bar");         // non-matching name is still filtered out

        // Assert
        // Per the INotifyPropertyChanged convention, null / empty PropertyName means "all properties changed",so both pass the filter while the unrelated "Bar" does not
        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.Same(source, r));
    }

    [Fact]
    public void AsValueObservableEmitsOnValueChange()
    {
        // Arrange
        var source = new TestNotificationValue<int>();
        var results = new List<int>();
        using var subscription = source.AsValueObservable().Subscribe(results.Add);

        // Act
        source.Value = 1;
        source.Value = 2;

        // Assert
        Assert.Equal([1, 2], results);
    }

    [Fact]
    public void AsValueObservableTreatsNullOrEmptyPropertyNameAsValueChange()
    {
        // Arrange
        var source = new TestNotificationValue<int> { Value = 7 };
        var results = new List<int>();
        using var subscription = source.AsValueObservable().Subscribe(results.Add);

        // Act
        source.Raise(null);          // null => all properties changed
        source.Raise(string.Empty);  // empty => all properties changed

        // Assert
        // null / empty PropertyName is treated as a Value change and emits the current value
        Assert.Equal([7, 7], results);
    }
}
