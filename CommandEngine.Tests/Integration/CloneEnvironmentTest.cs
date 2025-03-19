using CommandEngine.Commands;
using static CommandEngine.Tests.Util.PermanentStatus;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;
namespace CommandEngine.Tests.Integration
{
    public class CloneEnvironmentTest
    {
        [Fact]
        public void CloneSerialCommand()
        {
            var originalEnvironment = CommandEnvironment.Template("Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            ));
            var clonedEnvironment = CommandEnvironment.FreshEnvironment(originalEnvironment);
            Assert.Equal(originalEnvironment, clonedEnvironment);
            Assert.Equal(Succeeded, clonedEnvironment.Execute());
        }

        [Fact]
        public void CloneSerialWithSerialCommand()
        {
            var originalEnvironment = CommandEnvironment.Template("Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Inner Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful)
                ),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            ));
            var clonedEnvironment = CommandEnvironment.FreshEnvironment(originalEnvironment);
            Assert.Equal(originalEnvironment, clonedEnvironment);
        }
    }
}
