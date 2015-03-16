using System;
using System.Collections.Generic;
using System.IO;
using Overflow.Extensibility;
using Overflow.Utilities;

namespace Overflow
{
    /// <summary>
    /// A simple text logger. Writes an indented hierarchical representation
    /// of the executed operations and the behaviors that have been applied.
    /// </summary>
    public class TextWriterWorkflowLogger : IWorkflowLogger
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// Create new logger.
        /// </summary>
        /// <param name="writer">The text writer to send the output to. Use Console.Out
        /// to log to the console.</param>
        public TextWriterWorkflowLogger(TextWriter writer)
        {
            Verify.NotNull(writer, "writer");

            _writer = writer;
        }

        private readonly Stack<LevelInfo> _levelInfo = new Stack<LevelInfo>();

        /// <summary>
        /// Log that an operation has started executing.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        public void OperationStarted(IOperation operation)
        {
            Verify.NotNull(operation, "operation");

            PrepareForChildItem();

            _writer.Write(operation.GetType().Name);

            var levelInfo = new LevelInfo();
            _levelInfo.Push(levelInfo);
        }

        private void PrepareForChildItem()
        {
            if (_levelInfo.Count > 0)
            {
                if (_levelInfo.Peek().Children == 0)
                    _writer.Write(" {");

                _writer.WriteLine();

                if (_levelInfo.Peek().Children > 0)
                    _writer.WriteLine();

                _levelInfo.Peek().Children++;
            }
            AddNesting();
        }

        private void AddNesting()
        {
            _writer.Write(new string(' ', 2 * _levelInfo.Count));
        }

        /// <summary>
        /// Log an operation has finished executing.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        public void OperationFinished(IOperation operation)
        {
            Verify.NotNull(operation, "operation");
            Verify.Operation(_levelInfo.Count > 0, "No operation was logged as started so a finished operation cannot be logged.");

            var levelInfo = _levelInfo.Pop();

            if (levelInfo.Children > 0)
                _writer.Write(Environment.NewLine + "}");
        }

        /// <summary>
        /// Log an exception while executing an operation.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        /// <param name="error">The exception being thrown</param>
        public void OperationFailed(IOperation operation, Exception error)
        {
            Verify.NotNull(operation, "operation");
            Verify.NotNull(error, "error");
            Verify.Operation(_levelInfo.Count > 0, "No operation was logged as started so an operation failure cannot be logged.");

            PrepareForChildItem();

            _writer.Write("Error [" + error.GetType().Name + "]: " + error.Message);
        }

        /// <summary>
        /// Log that a behavior was applied, modifying the normal
        /// execution flow.
        /// </summary>
        /// <param name="operation">The executing operation</param>
        /// <param name="behavior">The beahvior being applied</param>
        /// <param name="description">A description of how the behavior modified the exectuion
        /// flow</param>
        public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description)
        {
            Verify.NotNull(operation, "operation");
            Verify.NotNull(behavior, "behavior");
            Verify.NotNull(description, "description");
            Verify.Operation(_levelInfo.Count > 0, "No operation was logged as started so an operation behavior cannot be logged.");
            
            PrepareForChildItem();

            _writer.Write(behavior.GetType().Name + ": " + description);
        }

        private class LevelInfo
        {
            public int Children { get; set; }
        }
    }
}
