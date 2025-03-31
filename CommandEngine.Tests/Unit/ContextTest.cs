using CommandEngine.Commands;
using CommandEngine.Tasks;
using CommandEngine.Tests.Util;
using Xunit.Abstractions;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandState;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tasks.CommandTask;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Commands.CommandEventType;
using static CommandEngine.Tests.Unit.TestConclusion;

namespace CommandEngine.Tests.Unit {
    public class ContextTest(ITestOutputHelper testOutput) {
        [Fact]
        public void SimpleConclusion() {
            var context = new Context();
            Assert.Throws<MissingContextInformationException>(() => context[Conclusion]);
            context[Conclusion] = NotPay;
            Assert.Equal(NotPay, context[Conclusion]);
        }

        [Fact]
        public void ResetContextValue() {
            var c = new Context();
            c[A] = "A";
            Assert.Equal("A", c[A]);
            Assert.True(c.Reset(A));
            Assert.False(c.Has(A));
                #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<InvalidOperationException>(() => c[A] = null);
                #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.False(c.Reset(B));
        }

        [Fact]
        public void TaskWithConclusion() {
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new ConclusionTask(NotPay).Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(AlwaysSuccessful)
            );
            var c = new Context();
            var e = Assert.Throws<ConclusionException>(() => command.Execute(c));
            Assert.Equal(NotPay, e.Conclusion);
            command.AssertStates(Executed, Executed, Initial);
            Assert.Single(c.History.Events(ConclusionReached));
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void TaskWithConclusionOnUndo() {
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                AlwaysSuccessful.Otherwise(new ConclusionTask(NotPay)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            var c = new Context();
            var e = Assert.Throws<ConclusionException>(() => command.Execute(c));
            Assert.Equal(NotPay, e.Conclusion);
            command.AssertStates(Executed, Executed, FailedToExecute);
            Assert.Single(c.History.Events(ConclusionReached));
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void ExtractSubContext() {
            var c = new Context {
                [A] = "A",
                [B] = "B",
                [C] = "C"
            };
            var sub = c.SubContext(Labels(A, B), []);
            Assert.Equal("A", sub[A]);
            Assert.Equal("B", sub[B]);
            Assert.Throws<MissingContextInformationException>(() => sub[C]);
        }

        [Fact]
        public void InvalidContextInExecuteTask() {
            var c = new Context {
                [A] = "A"
            };
            var command = "Primary Group".Sequence(
                new ReadTask(Labels(A, B)).NoReverting()
            );
            Assert.Equal(Failed, command.Execute(c));
            Assert.Single(c.History.Events(InvalidAccessAttempt));
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void InvalidContextInUndoTask() {
            var c = new Context {
                [A] = "A"
            };
            var command = "Primary Group".Sequence(
                AlwaysSuccessful.Otherwise(new ReadTask(Labels(A, B))),
                AlwaysFail.NoReverting()
            );
            Assert.Throws<UndoTaskFailureException>(() => command.Execute(c));
            Assert.Single(c.History.Events(InvalidAccessAttempt));
            testOutput.WriteLine(c.History.ToString());
        }

        [Fact]
        public void ContextUpdateAttemptInExecuteTask() {
            var c = Context(A, B);
            var command = "Primary Group".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new WriteTask(Labels(C, D)).Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(c));
            Assert.False(c.Has(C));
            Assert.False(c.Has(D));
            Assert.True(c.Has(A));
            Assert.True(c.Has(B));
            testOutput.WriteLine(c.History.ToString());
            Assert.Single(c.History.Events(UpdateNotCaptured));
        }

        [Fact]
        public void ContextUpdateAttemptInUndoTask() {
            var c = Context(A, B);
            var command = "Primary Group".Sequence(
                AlwaysSuccessful.Otherwise(new WriteTask(Labels(C, D))),
                AlwaysFail.NoReverting()
            );
            Assert.Throws<UndoTaskFailureException>(() => command.Execute(c));
            Assert.True(c.Has(A));
            Assert.True(c.Has(B));
            Assert.False(c.Has(C));
            Assert.False(c.Has(D));
            testOutput.WriteLine(c.History.ToString());
            Assert.Single(c.History.Events(UpdateNotCaptured));
        }

        [Fact]
        public void TaskWithSubContext() {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var command = "Primary Group".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<MissingContextInformationException>(() => c[D]);
            Assert.Equal(Succeeded, command.Execute(c));
            Assert.Equal("DChanged", c[D]);
            Assert.Equal("BChanged", c[B]);
            Assert.Equal(2, c.History.Events(ValueChanged).Count);
        }

        [Fact]
        public void UndoTaskWithSubContext() {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(new ContextTask(neededLabels, changedLabels, missingLabels)),
                AlwaysFail.Otherwise(AlwaysSuccessful)
            );
            Assert.Equal(Reverted, command.Execute(c));
            Assert.Equal("DChanged", c[D]);
            Assert.Equal("BChanged", c[B]);
        }

        [Fact]
        public void TaskWithMissingLabel() {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B, D);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                new ContextTask(neededLabels, changedLabels, missingLabels).Otherwise(AlwaysSuccessful)
            );
            Assert.Throws<MissingContextInformationException>(() => c[D]);
            Assert.Equal(Succeeded, command.Execute(c));
            Assert.Equal("DChanged", c[D]);
            Assert.Equal("BChanged", c[B]);
        }

        [Fact]
        public void TaskWithRequiredLabel() {
            var c = Context(A, B, C);
            var neededLabels = Labels(A, B, D);
            var changedLabels = Labels(D, B);
            var missingLabels = Labels(C);
            var command = "Master Sequence".Sequence(
                AlwaysSuccessful.Otherwise(AlwaysSuccessful),
                Mandatory(new ContextTask(neededLabels, changedLabels, missingLabels)).Otherwise(AlwaysSuccessful)
            );
            Assert.False(c.Has(D));
            Assert.Throws<TaskSuspendedException>(() => command.Execute(c));
            c[D] = D;
            Assert.Equal(Succeeded, command.Execute(c));
            Assert.Equal("DChanged", c[D]);
            Assert.Equal("BChanged", c[B]);
        }

        private static List<Enum> Labels(params Enum[] labels) => [.. labels];

        private static Context Context(params Enum[] labels) {
            var result = new Context();
            foreach (var label in labels) result[label] = label.ToString().ToUpper();
            return result;
        }
    }

    internal enum TestConclusion {
        Pay,
        NotPay,
        CallPolice
    }

    public enum TestLabels {
        Conclusion,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I
    }
}