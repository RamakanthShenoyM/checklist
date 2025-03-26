using CommandEngine.Commands;
using CommandEngine.Tasks;
using static CommandEngine.Commands.CommandReflection;

namespace CommandEngine.Tests.Unit {
    // Ensures Task reflection accurately assesses the Memento capabilities
    public class ReflectionTest {
        [Fact]
        public void NoMementoNecessary() {
            try {
                ValidateMementoStatus(new NoMementoNeeded().GetType());
            }
            catch (Exception e) {
                Assert.Fail($"Validation threw an unexpected exception of {e}");
            }
        }

        [Fact]
        public void MissingClone() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoClone().GetType()));
            Assert.Contains("required Clone()", e.Message);
        }

        [Fact]
        public void MissingToMemento() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoToMemento().GetType()));
            Assert.Contains("required ToMemento()", e.Message);
        }

        [Fact]
        public void MissingFromMemento() {
            var e = Assert.Throws<InvalidOperationException>(() => ValidateMementoStatus(new NoFromMemento().GetType()));
            Assert.Contains("required static FromMemento()", e.Message);
        }

        private abstract class BaseTask : CommandTask {
            public CommandStatus Execute(Context c) => throw new NotImplementedException();
            public List<Enum> NeededLabels => [];
            public List<Enum> ChangedLabels => [];
        }
        
        private class NoMementoNeeded: BaseTask {}

        private class NoClone : BaseTask {
            private int x = 4;
            public string ToMemento() => x.ToString();
            public static NoClone FromMemento(string memento) => new NoClone();
        }

        private class NoToMemento : BaseTask {
            private int x = 4;
            public NoToMemento Clone() => this;
            public static NoClone FromMemento(string memento) => new NoClone();
        }

        private class NoFromMemento : BaseTask {
            private int x = 4;
            public NoFromMemento Clone() => this;
            public string ToMemento() => x.ToString();
        }
    }
}