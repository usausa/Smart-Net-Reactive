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

    [Fact]
    public void AndWithEmptyOthersReturnsSameSource()
    {
        // Arrange
        using var s = new Subject<bool>();

        // Act & Assert
        // With no additional sources, And returns the source unchanged.
        Assert.Same(s, s.And());
    }

    [Fact]
    public void OrWithEmptyOthersReturnsSameSource()
    {
        // Arrange
        using var s = new Subject<bool>();

        // Act & Assert
        // With no additional sources, Or returns the source unchanged.
        Assert.Same(s, s.Or());
    }

    [Fact]
    public void AndWithManyOthersCombinesAll()
    {
        // Arrange
        // Five inputs (others.Length == 4) exercises the collection CombineLatest path.
        var results = new List<bool>();
        using var s1 = new Subject<bool>();
        using var s2 = new Subject<bool>();
        using var s3 = new Subject<bool>();
        using var s4 = new Subject<bool>();
        using var s5 = new Subject<bool>();
        s1.And(s2, s3, s4, s5).Subscribe(results.Add);

        // Act
        s1.OnNext(true);
        s2.OnNext(true);
        s3.OnNext(true);
        s4.OnNext(true);
        s5.OnNext(true);  // all true => true
        s5.OnNext(false); // one false => false

        // Assert
        Assert.Equal([true, false], results);
    }

    [Fact]
    public void OrWithManyOthersCombinesAll()
    {
        // Arrange
        // Five inputs (others.Length == 4) exercises the collection CombineLatest path.
        var results = new List<bool>();
        using var s1 = new Subject<bool>();
        using var s2 = new Subject<bool>();
        using var s3 = new Subject<bool>();
        using var s4 = new Subject<bool>();
        using var s5 = new Subject<bool>();
        s1.Or(s2, s3, s4, s5).Subscribe(results.Add);

        // Act
        s1.OnNext(false);
        s2.OnNext(false);
        s3.OnNext(false);
        s4.OnNext(false);
        s5.OnNext(false); // all false => false
        s5.OnNext(true);  // one true => true

        // Assert
        Assert.Equal([false, true], results);
    }
}
