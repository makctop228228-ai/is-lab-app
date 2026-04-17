namespace IsLabApp.Tests;

public class UnitTest1
{
    [Fact]
    public void PassingTest() => Assert.Equal(4, Add(2, 2));
    [Fact]
    public void AnotherPassingTest() => Assert.Equal(0, Add(0, 0));
    static int Add(int x, int y) => x + y;
}