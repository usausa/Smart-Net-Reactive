namespace Smart.Reactive;

using System.Reactive.Subjects;

public sealed class BooleanTest
{
    [Fact]
    public void NotInvertsValues()
    {
        // Arrange
        var results = new List<bool>();
        using var subject = new Subject<bool>();
        subject.Not().Subscribe(results.Add);

        // Act
        subject.OnNext(true);
        subject.OnNext(false);
        subject.OnCompleted();

        // Assert
        Assert.Equal([false, true], results);
    }

    [Fact]
    public void AndCombinesWithLogicalAnd()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.And(right).Subscribe(results.Add);

        // Act
        left.OnNext(true);
        right.OnNext(true);  // true && true = true
        right.OnNext(false); // true && false = false
        left.OnNext(false);  // false && false = false

        // Assert
        Assert.Equal([true, false, false], results);
    }

    [Fact]
    public void OrCombinesWithLogicalOr()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.Or(right).Subscribe(results.Add);

        // Act
        left.OnNext(false);
        right.OnNext(false); // false || false = false
        right.OnNext(true);  // false || true = true
        left.OnNext(false);  // false || true = true (right still true)

        // Assert
        Assert.Equal([false, true, true], results);
    }

    [Fact]
    public void XorCombinesWithLogicalXor()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.Xor(right).Subscribe(results.Add);

        // Act
        left.OnNext(true);
        right.OnNext(false); // true ^ false = true
        right.OnNext(true);  // true ^ true = false

        // Assert
        Assert.Equal([true, false], results);
    }

    [Fact]
    public void NandCombinesWithLogicalNand()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.Nand(right).Subscribe(results.Add);

        // Act
        left.OnNext(true);
        right.OnNext(true);  // !(true && true) = false
        right.OnNext(false); // !(true && false) = true

        // Assert
        Assert.Equal([false, true], results);
    }

    [Fact]
    public void NorCombinesWithLogicalNor()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.Nor(right).Subscribe(results.Add);

        // Act
        left.OnNext(false);
        right.OnNext(false); // !(false || false) = true
        right.OnNext(true);  // !(false || true) = false

        // Assert
        Assert.Equal([true, false], results);
    }

    [Fact]
    public void XnorCombinesWithLogicalXnor()
    {
        // Arrange
        var results = new List<bool>();
        using var left = new Subject<bool>();
        using var right = new Subject<bool>();
        left.Xnor(right).Subscribe(results.Add);

        // Act
        left.OnNext(true);
        right.OnNext(true);  // true == true = true
        right.OnNext(false); // true == false = false

        // Assert
        Assert.Equal([true, false], results);
    }

    [Fact]
    public void AndWithMultipleSourcesCombinesAll()
    {
        // Arrange
        var results = new List<bool>();
        using var s1 = new Subject<bool>();
        using var s2 = new Subject<bool>();
        using var s3 = new Subject<bool>();
        s1.And(s2, s3).Subscribe(results.Add);

        // Act
        s1.OnNext(true);
        s2.OnNext(true);
        s3.OnNext(true);  // true && true && true = true
        s3.OnNext(false); // true && true && false = false

        // Assert
        Assert.Equal([true, false], results);
    }

    [Fact]
    public void OrWithMultipleSourcesCombinesAll()
    {
        // Arrange
        var results = new List<bool>();
        using var s1 = new Subject<bool>();
        using var s2 = new Subject<bool>();
        using var s3 = new Subject<bool>();
        s1.Or(s2, s3).Subscribe(results.Add);

        // Act
        s1.OnNext(false);
        s2.OnNext(false);
        s3.OnNext(false); // false || false || false = false
        s3.OnNext(true);  // false || false || true = true

        // Assert
        Assert.Equal([false, true], results);
    }
}
