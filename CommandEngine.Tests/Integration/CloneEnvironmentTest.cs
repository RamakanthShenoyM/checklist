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
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        [Fact]
        public void CloneSerialWithSerialCommand()
        {
            var template = CommandEnvironment.Template("Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                "Inner Sequence".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    AlwaysFail.Otherwise(AlwaysSuccessful)
                ),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Reverted, originalEnvironment.Execute());
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }
    }
}
