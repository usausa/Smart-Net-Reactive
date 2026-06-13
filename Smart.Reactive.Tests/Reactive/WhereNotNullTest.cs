namespace Smart.Reactive;

using System.Reactive.Subjects;

public sealed class WhereNotNullTest
{
    [Fact]
    public void NullReferenceIsFiltered()
    {
        // Arrange
        var results = new List<string>();
        using var subject = new Subject<string?>();
        subject.WhereNotNull().Subscribe(results.Add);

        // Act
        subject.OnNext("hello");
        subject.OnNext(null);
        subject.OnNext("world");
        subject.OnCompleted();

        // Assert
        Assert.Equal(["hello", "world"], results);
    }

    [Fact]
    public void NonNullValuesPass()
    {
        // Arrange
        var results = new List<string>();
        using var subject = new Subject<string?>();
        subject.WhereNotNull().Subscribe(results.Add);

        // Act
        subject.OnNext("a");
        subject.OnNext("b");
        subject.OnCompleted();

        // Assert
        Assert.Equal(["a", "b"], results);
    }

    [Fact]
    public void AllNullsProduceNoOutput()
    {
        // Arrange
        var results = new List<string>();
        using var subject = new Subject<string?>();
        subject.WhereNotNull().Subscribe(results.Add);

        // Act
        subject.OnNext(null);
        subject.OnNext(null);
        subject.OnCompleted();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void NullableValueTypeNullIsFiltered()
    {
        // Arrange
        var results = new List<int>();
        using var subject = new Subject<int?>();
        subject.WhereNotNull().Subscribe(results.Add);

        // Act
        subject.OnNext(1);
        subject.OnNext(null);
        subject.OnNext(3);
        subject.OnCompleted();

        // Assert
        Assert.Equal([1, 3], results);
    }

    [Fact]
    public void NullableValueTypeNonNullsPass()
    {
        // Arrange
        var results = new List<int>();
        using var subject = new Subject<int?>();
        subject.WhereNotNull().Subscribe(results.Add);

        // Act
        subject.OnNext(42);
        subject.OnNext(null);
        subject.OnNext(99);
        subject.OnCompleted();

        // Assert
        Assert.Equal([42, 99], results);
    }
}
