namespace SpecFlow.Tests;
public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10};

        var resultSequence = numbers[4..]; // Sequence
        Assert.Contains(5, resultSequence);
        Assert.Contains(6, resultSequence);
        Assert.Contains(7, resultSequence);
        Assert.Contains(8, resultSequence);
        var resultIndexed = numbers[^2];
        Assert.Equal(9, resultIndexed);
    }
}