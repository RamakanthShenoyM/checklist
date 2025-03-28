using CommandEngine.Commands;
using CommandEngine.Tasks;
using CommandEngine.Tests.Util;
using Xunit.Abstractions;
using static CommandEngine.Commands.CommandEventType;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Tests.Util.TestExtensions;
using static CommandEngine.Tests.Util.SuspendLabels;

namespace CommandEngine.Tests.Integration {
    public class CloneEnvironmentTest(ITestOutputHelper testOutput) {
        [Fact]
        public void CloneSerialCommand() {
            var template = "Incident process one".Template("Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var memento = originalEnvironment.ToMemento();
            testOutput.WriteLine(memento);
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        [Fact]
        public void CloneSerialWithSerialCommand() {
            var template = "Incident process one".Template("Master Sequence".Sequence(
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
            var memento = originalEnvironment.ToMemento();
            testOutput.WriteLine(memento);
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        [Fact]
        public void CloneWithSubContext() {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var template = "Incident process one".Template("Primary Group".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var memento = originalEnvironment.ToMemento();
            testOutput.WriteLine(memento);
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        [Fact]
        public void VariousContextTypes() {
            var c = Context();
            c[A] = "A";
            c[B] = 1990;
            c[C] = 3.14;
            c[D] = true;
            c[E] = 'e';
            c[F] = new DateTime(2021, 1, 1);

            var template = "Incident process one".Template("Primary Group".Sequence(
                AlwaysSuccessful.NoReverting()
            ));
            var originalEnvironment = CommandEnvironment.FreshEnvironment(template, c);
            Assert.Equal(Succeeded, originalEnvironment.Execute());
            var memento = originalEnvironment.ToMemento();
            testOutput.WriteLine(memento);
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
            Assert.Equal(originalEnvironment, restoredEnvironment);
        }

        [Fact]
        public void SuspendSaveRestoreResume() {
            var template = "Incident process one".Template("Primary Group".Sequence(
                AlwaysSuccessful.NoReverting(),
                new SuspendFirstOnly().NoReverting(),
                AlwaysSuccessful.NoReverting()
            ));
            var firstUse = CommandEnvironment.FreshEnvironment(template);
            Assert.Throws<TaskSuspendedException>(() => firstUse.Execute());
            var secondUse = CommandEnvironment.FreshEnvironment(template);
            Assert.Throws<TaskSuspendedException>(() => secondUse.Execute());
        }

        [Fact]
        public void CloneTaskWithState() {
            var template = "Incident process one".Template("Primary Group".Sequence(
               new CountingTask().NoReverting()
            ));
            var redEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, redEnvironment.Execute());
            Assert.Equal(1, redEnvironment[CountingTaskCount]);
            var blueEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, blueEnvironment.Execute());
            Assert.Equal(1, blueEnvironment[CountingTaskCount]);

        }
        [Fact]
        public void SaveTaskWithState()
        {
            var template = "Incident process one".Template("Primary Group".Sequence(
                new CountingTask().NoReverting()
            ));
            var redEnvironment = CommandEnvironment.FreshEnvironment(template);
            Assert.Equal(Succeeded, redEnvironment.Execute());
            var memento = redEnvironment.ToMemento();
            var restoredEnvironment = CommandEnvironment.FromMemento(memento);
            Assert.Equal(redEnvironment,restoredEnvironment);

        }

        [Fact]
        public void StaticAnalysis()
        {
            var c = Context(A, B, D);
            var command = "Primary Group".Sequence(
                new ContextTask(Labels(A, B), Labels(D), Labels()).NoReverting(),
                new ContextTask(Labels(A), Labels(E), Labels()).NoReverting(),
                new ContextTask(Labels(A, E), Labels(D), Labels()).NoReverting(),
                new ContextTask(Labels(G), Labels(), Labels()).NoReverting()
            );
            var template = "Incident process one".Template(command);
            CommandEnvironment.FreshEnvironment(template, c);
            testOutput.WriteLine(c.History.ToString());
            Assert.Single(c.History.Events(OutSideLabels));
            Assert.Single(c.History.Events(WrittenLabels));
            Assert.Single(c.History.Events(SetAndUsedLabels));

        }

        [Fact]
        public void LabelSetAndUsed()
        {
            var c = Context(A, B);
            var command = "Primary Group".Sequence(
                new ContextTask(Labels(A, B), Labels(F), Labels()).NoReverting(),
                new ContextTask(Labels(F,G), Labels(E), Labels()).NoReverting(),
                new ContextTask(Labels(A, E), Labels(D), Labels()).NoReverting()
            );
            var template = "Incident process one".Template(command);
            CommandEnvironment.FreshEnvironment(template, c);
            testOutput.WriteLine(c.History.ToString());
            Assert.Single(c.History.Events(OutSideLabels));
            Assert.Single(c.History.Events(WrittenLabels));
            Assert.Single(c.History.Events(SetAndUsedLabels));
        }
        [Fact]
        public void NeededButSetLater()
        {
            var c = Context(A, B);
            var command = "Primary Group".Sequence(
                new ContextTask(Labels(A,E), Labels(), Labels()).NoReverting(),
                new ContextTask(Labels(F), Labels(E,B), Labels()).NoReverting(),
                new ContextTask(Labels(), Labels(F), Labels()).NoReverting());
            var template = "Incident process one".Template(command);
            CommandEnvironment.FreshEnvironment(template, c);
            testOutput.WriteLine(c.History.ToString());
           Assert.Single(c.History.Events(NeededLabelBeforeSet));
        }
    }
}