using CommandEngine.Tasks;
using static CommandEngine.Tests.Unit.TestConclusion;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Commands.SerialCommand;
using static CommandEngine.Tests.Util.PermanentStatus;
using CommandEngine.Tests.Util;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Commands.CommandState;

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
        [Fact]
        public void TaskWithConclusion()
        {
            var command = Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new ConclusionTask(NotPay).Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
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
