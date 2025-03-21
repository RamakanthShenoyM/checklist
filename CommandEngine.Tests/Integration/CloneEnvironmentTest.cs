﻿using CommandEngine.Commands;
using CommandEngine.Tasks;
using CommandEngine.Tests.Util;
using Xunit.Abstractions;
using static CommandEngine.Tests.Util.PermanentStatus;
using static CommandEngine.Commands.CommandStatus;
using static CommandEngine.Tests.Unit.TestLabels;
using static CommandEngine.Tests.Util.TestExtensions;

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
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
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
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
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
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
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
            var json = originalEnvironment.ToJson();
            testOutput.WriteLine(json);
            var restoredEnvironment = json.ToCommandEnvironment();
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

        private static Context Context(params Enum[] labels) {
            var result = new Context();
            foreach (var label in labels) result[label] = label.ToString().ToUpper();
            return result;
        }
    }
}