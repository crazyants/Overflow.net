using Overflow.Behaviors;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;

namespace Overflow.Test.Behaviors
{
    public class OperationLoggingBehaviorFactoryTests : TestBase
    {
        [Theory, AutoMoqData]
        public void The_logging_behaviors_are_created_when_the_log_is_defined_on_the_configuration(IWorkflowLogger logger, IOperation operation)
        {
            var sut = new OperationLoggingBehaviorFactory();
            var configuration = new FakeWorkflowConfiguration { Logger = logger };

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(2, result.Count);
            Assert.IsType<OperationExecutionLoggingBehavior>(result[0]);
            Assert.IsType<OperationErrorLoggingBehavior>(result[1]);
        }

        [Theory, AutoMoqData]
        public void The_logging_behavior_is_not_created_when_the_configuration_has_not_defined_a_logger_to_use(IOperation operation, WorkflowConfiguration configuration)
        {
            var sut = new OperationLoggingBehaviorFactory();

            var result = sut.CreateBehaviors(operation, configuration);

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void Guards_are_verified()
        {
            VerifyGuards<OperationLoggingBehaviorFactory>();
        }
    }
}
