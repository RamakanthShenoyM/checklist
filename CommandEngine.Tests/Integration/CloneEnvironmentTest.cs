using CommandEngine.Commands;
using static CommandEngine.Tests.Util.PermanentStatus;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandStatus;
using Xunit.Sdk;
using Xunit.Abstractions;
namespace CommandEngine.Tests.Integration
{
    public class CloneEnvironmentTest(ITestOutputHelper testOutput)
    {
        [Fact]
        public void CloneSerialCommand()
        {
            var template = CommandEnvironment.Template("Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template);
            var clonedEnvironment = CommandEnvironment.RestoredEnvironment(template, originalEnvironment.ClientId, new Context());
            Assert.Equal(originalEnvironment, clonedEnvironment);
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
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
