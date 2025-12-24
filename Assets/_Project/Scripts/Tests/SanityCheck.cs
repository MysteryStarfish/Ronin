using NUnit.Framework;

namespace Ronin.Tests
{
    public class SanityCheck
    {
        // 這是一個 "Edit Mode" 測試，不需要進入 Play 模式就能跑
        [Test]
        public void Environment_Is_Setup_Correctly()
        {
            // Arrange (準備)
            int a = 1;
            int b = 1;

            // Act (執行)
            int sum = a + b;

            // Assert (驗證)
            Assert.AreEqual(2, sum);
        }
    }
}