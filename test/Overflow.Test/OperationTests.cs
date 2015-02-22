using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class OperationTests
    {
        [Fact]
        public void Executing_an_operation_calls_the_OnExecute_method()
        {
            var sut = new FakeOperation();

            sut.Execute();
            
            Assert.True(sut.HasExecuted);
        }

        [Fact]
        public void Operations_do_not_have_any_child_operations_by_default()
        {
            var sut = new TestOperation();

            var result = sut.GetChildOperations();

            Assert.False(result.Any());
        }

        [Fact]
        public void Executing_an_operation_executes_each_child_operation_as_well()
        {
            var op1 = new FakeOperation();
            var op2 = new FakeOperation();
            var sut = new FakeOperation(op1, op2);

            sut.Execute();

            Assert.Equal(3, FakeOperation.ExecutedOperations.Count);
            Assert.Equal(sut, FakeOperation.ExecutedOperations[0]);
            Assert.Equal(op1, FakeOperation.ExecutedOperations[1]);
            Assert.Equal(op2, FakeOperation.ExecutedOperations[2]);
        }

        [Fact]
        public void The_operation_resolver_is_the_same_as_the_one_you_assign_to_it()
        {
            var resolver = new SimpleOperationResolver();
            Operation.Resolver = resolver;

            var result = Operation.Resolver;

            Assert.Equal(resolver, result);
        }

        [Fact]
        public void You_can_create_operations_when_the_operation_resolver_is_set()
        {
            Operation.Resolver = new SimpleOperationResolver();

            var result = Operation.Create<TestOperation>();

            Assert.NotNull(result);
        }

        [Fact]
        public void You_cannot_create_operations_when_the_operation_resolver_is_not_set()
        {
            Operation.Resolver = null;

            Assert.Throws<InvalidOperationException>(() => Operation.Create<TestOperation>());
        }

        [Fact]
        public void Data_flows_between_child_operations()
        {
            var inputOperation = new FakeInputOperation<object>();
            var outpuOperation = new FakeOutputOperation<object> { OutputValue = new object() };
            var sut = new FakeOperation(outpuOperation, inputOperation);

            sut.Execute();

            Assert.Equal(outpuOperation.OutputValue, inputOperation.ProvidedInput);
        }

        [Fact]
        public void You_can_get_outputted_values_from_within_the_operation()
        {
            var sut = new OutputtingOperation { ExpectedOutput = new object() };

            sut.Execute();

            Assert.Equal(sut.ExpectedOutput, sut.ActualOutput);
        }

        private class TestOperation : Operation {
            protected override void OnExecute() { }
        }

        private class OutputtingOperation : Operation
        {
            public object ExpectedOutput { get; set; }
            public object ActualOutput { get; private set; }

            protected override void OnExecute() { }

            public override IEnumerable<IOperation> GetChildOperations()
            {
                yield return new FakeOutputOperation<object> { OutputValue = ExpectedOutput };
                ActualOutput = GetChildOutputValue<object>();
            }
        }
    }
}
