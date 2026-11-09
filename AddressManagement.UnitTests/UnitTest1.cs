namespace AddressManagement.UnitTests
{
    public class UnitTest1
    {
       
        [Fact]
        public void FirstTest_Addition_AssertCorrectResult()
        {
            // Arrange
            int a = 5;
            int b = 7;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(12, result);
        }
    }
}
