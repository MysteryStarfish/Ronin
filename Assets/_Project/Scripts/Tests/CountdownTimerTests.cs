using NUnit.Framework;
using Ronin.Core;

namespace Ronin.Tests
{
    [TestFixture]
    public class CountdownTimerTests
    {
        [Test]
        public void Tick_WhenTimeExpires_ShouldInvokeOnStop()
        {
            bool finished = false;
            CountdownTimer timer = new CountdownTimer(5f);
            timer.OnStop += () => finished = true;
            timer.Start();
            timer.Tick(5f);
            Assert.IsTrue(finished);
        }
    }
}