using CommandEngine.Commands;
using CommandEngine.Tasks;
using Xunit.Abstractions;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using CommandEngine.Tests.Util;
using static CommandEngine.Tests.Unit.TestLabels;

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

        [Fact]
        public void CloneWithSubContext()
        {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var template = CommandEnvironment.Template("Primary Group".Sequence(
                    AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                    new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        private static List<Enum> Labels(params Enum[] labels) => [.. labels];
        private static Context Context(params Enum[] labels)
        {
            var result = new Context();
            foreach (var label in labels) result[label] = label.ToString().ToUpper();
            return result;
        }
    }
}
