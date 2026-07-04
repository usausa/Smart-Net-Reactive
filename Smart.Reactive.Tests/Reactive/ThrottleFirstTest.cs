namespace Smart.Reactive;

using System.Reactive.Concurrency;

using Microsoft.Reactive.Testing;

public sealed class ThrottleFirstTest : ReactiveTest
{
    // RecordingScheduler wraps an IScheduler and records Dispose calls on scheduled timers
    private sealed class RecordingScheduler : IScheduler
    {
        internal sealed class RecordingDisposable : IDisposable
        {
            private readonly IDisposable inner;
            private readonly RecordingScheduler owner;

            public bool IsDisposed { get; private set; }

            public RecordingDisposable(IDisposable inner, RecordingScheduler owner)
            {
                this.inner = inner;
                this.owner = owner;
            }

            public void Dispose()
            {
                IsDisposed = true;
                owner.DisposeCount++;
                inner.Dispose();
            }
        }

        private readonly IScheduler inner;

        // ReSharper disable once MemberCanBePrivate.Local
        public int DisposeCount { get; private set; }

        public List<RecordingDisposable> Scheduled { get; } = [];

        public DateTimeOffset Now => inner.Now;

        public RecordingScheduler(IScheduler inner)
        {
            this.inner = inner;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var d = inner.Schedule(state, action);
            var recording = new RecordingDisposable(d, this);
            Scheduled.Add(recording);
            return recording;
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var d = inner.Schedule(state, dueTime, action);
            var recording = new RecordingDisposable(d, this);
            Scheduled.Add(recording);
            return recording;
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var d = inner.Schedule(state, dueTime, action);
            var recording = new RecordingDisposable(d, this);
            Scheduled.Add(recording);
            return recording;
        }
    }

    [Fact]
    public void FirstElementPasses()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        Assert.Equal([OnNext(210, 1)], results.Messages);
    }

    [Fact]
    public void SubsequentWithinWindowAreSuppressed()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnNext(250, 2),
            OnNext(290, 3));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        // Only the first element (210) passes; 250 and 290 are within the window [210, 310)
        Assert.Equal([OnNext(210, 1)], results.Messages);
    }

    [Fact]
    public void ElementAfterWindowExpiryPasses()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnNext(350, 2));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        // 210 passes, window closes at 310, 350 > 310 so it passes too
        Assert.Equal([OnNext(210, 1), OnNext(350, 2)], results.Messages);
    }

    [Fact]
    public void BoundaryExactTickBehavior()
    {
        // When an element arrives at exactly the same tick as the window expires (tick 310),
        // the TestScheduler processes the hot-observable OnNext before the scheduled timer
        // callback, because the hot observable's messages were registered in the queue before
        // scheduler.Schedule was called from within the OnNext(210) handler
        //
        // Observed result: the element at tick 310 is BLOCKED (open = false when element arrives)
        // The source OnNext at 310 is dequeued before the window-open timer callback at 310,
        // so open is still false when value 2 arrives. This boundary behavior is deterministic
        // with TestScheduler virtual time: boundary-tick elements fall inside the window

        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnNext(310, 2));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        // At tick 310: the source element arrives while open=false => suppressed
        // The window-open timer at 310 fires after (or concurrently with lower priority), so the boundary element does NOT pass
        Assert.Equal([OnNext(210, 1)], results.Messages);
    }

    [Fact]
    public void OnErrorPropagates()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var error = new InvalidOperationException("test error");
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnError<int>(300, error));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        Assert.Equal(2, results.Messages.Count);
        Assert.Equal(OnNext(210, 1), results.Messages[0]);
        Assert.Equal(OnError<int>(300, error), results.Messages[1]);
    }

    [Fact]
    public void OnCompletedPropagates()
    {
        // Arrange
        var scheduler = new TestScheduler();
        var source = scheduler.CreateHotObservable(
            OnNext(210, 1),
            OnCompleted<int>(400));

        // Act
        var results = scheduler.Start(() =>
            source.ThrottleFirst(TimeSpan.FromTicks(100), scheduler));

        // Assert
        Assert.Equal([OnNext(210, 1), OnCompleted<int>(400)], results.Messages);
    }

    [Fact]
    public void DisposeUnsubscribesTimer()
    {
        // Verify that when the subscription is disposed, the pending timer is also disposed
        // This is the core of the fix: CompositeDisposable(subscription, timer)

        // Arrange
        var testScheduler = new TestScheduler();
        var recordingScheduler = new RecordingScheduler(testScheduler);

        var source = testScheduler.CreateHotObservable(
            OnNext(210, 1));

        IDisposable? subscription = null;

        testScheduler.Schedule(TimeSpan.FromTicks(200), () =>
        {
            subscription = source
                .ThrottleFirst(TimeSpan.FromTicks(100), recordingScheduler)
                .Subscribe(_ => { });
        });

        // Act
        // Advance past the first element to trigger the timer schedule
        testScheduler.AdvanceTo(220);

        // Assert
        // After the first element at 210, exactly one timer should have been scheduled
        Assert.Single(recordingScheduler.Scheduled);

        // The timer has not been disposed yet
        Assert.False(recordingScheduler.Scheduled[0].IsDisposed);

        // Act
        // Dispose the subscription
        subscription!.Dispose();

        // Assert
        // The timer disposable should now have been disposed via CompositeDisposable
        Assert.True(recordingScheduler.Scheduled[0].IsDisposed);
    }

    [Fact]
    public void OnCompletedDisposesPendingTimer()
    {
        // Verify that when the source completes while a window timer is still pending, the terminal notification disposes that timer via CompositeDisposable

        // Arrange
        var testScheduler = new TestScheduler();
        var recordingScheduler = new RecordingScheduler(testScheduler);

        var source = testScheduler.CreateHotObservable(
            OnNext(210, 1),
            OnCompleted<int>(250));

        IDisposable? subscription = null;

        testScheduler.Schedule(TimeSpan.FromTicks(200), () =>
        {
            subscription = source
                .ThrottleFirst(TimeSpan.FromTicks(100), recordingScheduler)
                .Subscribe(_ => { });
        });

        // Act
        // 210 schedules a window timer due at 310; OnCompleted fires at 250, before the window closes
        testScheduler.AdvanceTo(260);

        // Assert
        Assert.Single(recordingScheduler.Scheduled);
        Assert.True(recordingScheduler.Scheduled[0].IsDisposed);

        subscription!.Dispose();
    }

    [Fact]
    public void OnErrorDisposesPendingTimer()
    {
        // Verify that when the source errors while a window timer is still pending, the terminal notification disposes that timer via CompositeDisposable

        // Arrange
        var testScheduler = new TestScheduler();
        var recordingScheduler = new RecordingScheduler(testScheduler);
        var error = new InvalidOperationException("test error");

        var source = testScheduler.CreateHotObservable(
            OnNext(210, 1),
            OnError<int>(250, error));

        IDisposable? subscription = null;

        testScheduler.Schedule(TimeSpan.FromTicks(200), () =>
        {
            subscription = source
                .ThrottleFirst(TimeSpan.FromTicks(100), recordingScheduler)
                .Subscribe(_ => { }, _ => { });
        });

        // Act
        // 210 schedules a window timer due at 310; OnError fires at 250, before the window closes
        testScheduler.AdvanceTo(260);

        // Assert
        Assert.Single(recordingScheduler.Scheduled);
        Assert.True(recordingScheduler.Scheduled[0].IsDisposed);

        subscription!.Dispose();
    }
}
