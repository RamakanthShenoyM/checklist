using CommandEngine.Tasks;
using static CommandEngine.Tests.Unit.TestConclusion;
using static CommandEngine.Tests.Unit.TestLabels;

namespace CommandEngine.Tests.Unit
{
    public class ContextTest
    {
        [Fact]
        public void SimpleConclusion()
        {
            var context = new Context();
            Assert.Throws<MissingContextInformationException>(() => context[Conclusion]);
            context[Conclusion] = NotPay;
            Assert.Equal(NotPay, context[Conclusion]);
        }


    }
    internal enum TestConclusion
    {
        Pay,
        NotPay,
        CallPolice
    }

    internal enum TestLabels
    {
        Conclusion
    }
}
