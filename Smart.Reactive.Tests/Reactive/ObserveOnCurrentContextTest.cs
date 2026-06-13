namespace Smart.Reactive;

using System.Reactive.Subjects;

public sealed class ObserveOnCurrentContextTest
{
    // A test SynchronizationContext that records Post calls and executes them synchronously.
    private sealed class RecordingSynchronizationContext : SynchronizationContext
    {
        public int PostCount { get; private set; }

        public override void Post(SendOrPostCallback d, object? state)
        {
            PostCount++;
            d(state);
        }
    }

    [Fact]
    public void DeliveryGoesthroughSynchronizationContext()
    {
        // Arrange
        var previousContext = SynchronizationContext.Current;
        var testContext = new RecordingSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(testContext);

        try
        {
            var results = new List<int>();
            using var subject = new Subject<int>();

            // ObserveOnCurrentContext captures the current context at subscription time
            subject.ObserveOnCurrentContext().Subscribe(results.Add);

            // Act
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            // Assert
            Assert.Equal([1, 2, 3], results);

            // Each OnNext was delivered via Post
            Assert.True(testContext.PostCount >= 3, $"Expected at least 3 Post calls, got {testContext.PostCount}");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    [Fact]
    public void ThrowsWhenNoCurrentContext()
    {
        // Arrange
        var previousContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);

        try
        {
            using var subject = new Subject<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(subject.ObserveOnCurrentContext);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }
}
