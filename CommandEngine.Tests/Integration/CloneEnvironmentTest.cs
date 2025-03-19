using CommandEngine.Commands;
using static CommandEngine.Tests.Util.PermanentStatus;
using CommandEngine.Tasks;
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
        }
    }
}
