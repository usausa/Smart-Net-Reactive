namespace Smart.Reactive;

using System.Reactive;

using Microsoft.Reactive.Testing;

public sealed class PairwiseTest : ReactiveTest
{
    [Fact]
    public void SingleElementProducesNoPairs()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1));

        // Act
        var results = scheduler.Start(() =>
            source.Pairwise((prev, current) => (prev, current)));

        // Assert
        Assert.Empty(results.Messages);
    }

    [Fact]
    public void SecondElementProducesFirstPair()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnNext(220, 2));

        // Act
        var results = scheduler.Start(() =>
            source.Pairwise((prev, current) => (prev, current)));

        // Assert
        Assert.Equal(1, results.Messages.Count);
        Assert.Equal(220, results.Messages[0].Time);
        Assert.Equal((1, 2), results.Messages[0].Value.Value);
    }

    [Fact]
    public void MultipleElementsProduceSlidingPairs()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 10),
            OnNext(220, 20),
            OnNext(230, 30),
            OnNext(240, 40));

        // Act
        var results = scheduler.Start(() =>
            source.Pairwise((prev, current) => (prev, current)));

        // Assert
        Assert.Equal(3, results.Messages.Count);
        Assert.Equal((10, 20), results.Messages[0].Value.Value);
        Assert.Equal((20, 30), results.Messages[1].Value.Value);
        Assert.Equal((30, 40), results.Messages[2].Value.Value);
    }

    [Fact]
    public void OnCompletedPropagates()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnCompleted<int>(300));

        // Act
        var results = scheduler.Start(() =>
            source.Pairwise((prev, current) => (prev, current)));

        // Assert
        Assert.Single(results.Messages);
        Assert.Equal(NotificationKind.OnCompleted, results.Messages[0].Value.Kind);
        Assert.Equal(300, results.Messages[0].Time);
    }

    [Fact]
    public void OnErrorPropagates()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var error = new InvalidOperationException("test");
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnError<int>(300, error));

        // Act
        var results = scheduler.Start(() =>
            source.Pairwise((prev, current) => (prev, current)));

        // Assert
        Assert.Single(results.Messages);
        Assert.Equal(NotificationKind.OnError, results.Messages[0].Value.Kind);
        Assert.Equal(error, results.Messages[0].Value.Exception);
    }
}
