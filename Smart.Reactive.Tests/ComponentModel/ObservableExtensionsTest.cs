namespace Smart.ComponentModel;

using System.Reactive.Subjects;

public sealed class ObservableExtensionsTest
{
    private sealed class ValueHolder<T> : IValueHolder<T>
    {
        public T Value { get; set; } = default!;
    }

    [Fact]
    public void SubscribeValueSetsHolderValue()
    {
        // Arrange
        using var subject = new Subject<int>();
        var holder = new ValueHolder<int>();
        using var subscription = subject.SubscribeValue(holder);

        // Act
        subject.OnNext(7);

        // Assert
        Assert.Equal(7, holder.Value);
    }

    [Fact]
    public void SubscribeValueWithSelectorSetsHolderValue()
    {
        // Arrange
        using var subject = new Subject<int>();
        var holder = new ValueHolder<int>();
        using var subscription = subject.SubscribeValue(static x => x * 2, holder);

        // Act
        subject.OnNext(21);

        // Assert
        Assert.Equal(42, holder.Value);
    }

    [Fact]
    public void SubscribeValueSelectorExceptionPropagates()
    {
        // Arrange
        using var subject = new Subject<int>();
        var holder = new ValueHolder<int>();
        var error = new InvalidOperationException("selector");
        using var subscription = subject.SubscribeValue(x => x == 0 ? throw error : x * 2, holder);

        // Act & Assert
        // The selector throws inside the observer's OnNext; the exception surfaces to the OnNext caller
        var ex = Assert.Throws<InvalidOperationException>(() => subject.OnNext(0));
        Assert.Same(error, ex);
    }
}
