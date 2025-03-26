using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandReflection;
// ReSharper disable UnusedMember.Local
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace CommandEngine.Tests.Unit {
    // Ensures Task reflection accurately assesses the Memento capabilities
    public class ReflectionTest {
        [Fact]
        public void NoMementoNecessary() {
            try {
                ValidateMementoStatus(new NoMementoNeeded().GetType());
                ValidateMementoStatus(new PerfectMementoTask().GetType());
            }
            catch (Exception e) {
                Assert.Fail($"Validation threw an unexpected exception of {e}");
            }
        }

        [Fact]
        public void MissingClone() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoClone().GetType()));
            Assert.Contains("required MementoTask interface", e.Message);
        }

        [Fact]
        public void MissingToMemento() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoToMemento().GetType()));
            Assert.Contains("required MementoTask interface", e.Message);
        }

        [Fact]
        public void MissingFromMemento() {
            var e = Assert.Throws<InvalidOperationException>(() =>
                ValidateMementoStatus(new NoFromMemento().GetType()));
            Assert.Contains("required static FromMemento()", e.Message);
        }

        [Fact]
        public void MissingEqualsOverride() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoEquals().GetType()));
            Assert.Contains("required override of Equals()", e.Message);
        }

        private abstract class BaseTask : CommandTask {
            public CommandStatus Execute(Context c) => throw new NotImplementedException();
            public List<Enum> NeededLabels => [];
            public List<Enum> ChangedLabels => [];
        }

        private class NoMementoNeeded : BaseTask { }

        private class NoClone : BaseTask {
            private readonly int _x = 4;
            public string ToMemento() => _x.ToString();
            public static NoClone FromMemento(string _) => new();
            public override bool Equals(object? obj) =>
                this == obj || obj is NoClone other && this._x == other._x;
        }

        private class NoToMemento : BaseTask {
            private readonly int _x = 4;
            public CommandTask Clone() => this;
            public static NoToMemento FromMemento(string _) => new();
            public override bool Equals(object? obj) =>
                this == obj || obj is NoToMemento other && this._x == other._x;
        }

        private class NoFromMemento : BaseTask, MementoTask {
            private readonly int _x = 4;
            public CommandTask Clone() => this;
            public string ToMemento() => _x.ToString();
            public override bool Equals(object? obj) =>
                this == obj || obj is NoFromMemento other && this._x == other._x;
        }

        private class NoEquals : BaseTask, MementoTask {
            private readonly int _x = 4;
            public CommandTask Clone() => this;
            public string ToMemento() => _x.ToString();
            public static NoEquals FromMemento(string _) => new();
        }

        private class PerfectMementoTask : BaseTask, MementoTask {
            private readonly int _x = 4;
            public CommandTask Clone() => this;
            public string ToMemento() => _x.ToString();
            public static PerfectMementoTask FromMemento(string _) => new();
            public override bool Equals(object? obj) =>
                this == obj || obj is PerfectMementoTask other && this._x == other._x;
        }
    }
}