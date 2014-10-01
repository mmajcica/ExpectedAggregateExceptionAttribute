using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExceptionTesting.Tests
{
    /// <summary>
    /// Indicates that an exception is expected during test method execution.
    /// It also considers the AggregateException and check if the given exception is contained inside the InnerExceptions.
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ExpectedAggregateExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        protected override string NoExceptionMessage
        {
            get
            {
                return string.Format("{0} - {1}, {2}, {3}",
                    this.TestContext.FullyQualifiedTestClassName,
                    this.TestContext.TestName,
                    this.ExceptionType.FullName,
                    base.NoExceptionMessage);
            }
        }

        /// <summary>
        /// Gets the expected exception type.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Type"/> object.
        /// </returns>
        public Type ExceptionType { get; private set; }

        public bool AllowDerivedTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedAggregateExceptionAttribute"/> class with and expected exception type and a message that describes the exception.
        /// </summary>
        /// <param name="exceptionType">An expected type of exception to be thrown by a method.</param>
        public ExpectedAggregateExceptionAttribute(Type exceptionType)
            : this(exceptionType, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedAggregateExceptionAttribute"/> class with and expected exception type and a message that describes the exception.
        /// </summary>
        /// <param name="exceptionType">An expected type of exception to be thrown by a method.</param>
        /// <param name="noExceptionMessage">If the test fails because an exception was not thrown, this message is included in the test result.</param>
        public ExpectedAggregateExceptionAttribute(Type exceptionType, string noExceptionMessage)
            : base(noExceptionMessage)
        {
            if (exceptionType == null)
            {
                throw new ArgumentNullException("exceptionType");
            }

            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new ArgumentException("Given type is not an exception.", "exceptionType");
            }

            this.ExceptionType = exceptionType;
        }

        /// <param name="exception">The exception that is thrown by the unit test.</param>
        protected override void Verify(Exception exception)
        {
            Type type = exception.GetType();

            if (this.AllowDerivedTypes)
            {
                if (!this.ExceptionType.IsAssignableFrom(type))
                {
                    base.RethrowIfAssertException(exception);

                    throw new Exception(string.Format("Test method {0}.{1} threw exception {2}, but exception {3} was expected. Exception message: {4}",
                        base.TestContext.FullyQualifiedTestClassName,
                        base.TestContext.TestName,
                        type.FullName,
                        this.ExceptionType.FullName,
                        GetExceptionMsg(exception)));
                }
            }
            else
            {
                if (type == typeof(AggregateException))
                {
                    foreach (var e in ((AggregateException)exception).InnerExceptions)
                    {
                        if (e.GetType() == this.ExceptionType)
                        {
                            return;
                        }
                    }
                }

                if (type != this.ExceptionType)
                {
                    base.RethrowIfAssertException(exception);

                    throw new Exception(string.Format("Test method {0}.{1} threw exception {2}, but exception {3} was expected. Exception message: {4}",
                        base.TestContext.FullyQualifiedTestClassName,
                        base.TestContext.TestName,
                        type.FullName,
                        this.ExceptionType.FullName,
                        GetExceptionMsg(exception)));
                }
            }
        }

        private string GetExceptionMsg(Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;

            for (Exception exception = ex; exception != null; exception = exception.InnerException)
            {
                string str = exception.Message;

                FileNotFoundException notFoundException = exception as FileNotFoundException;
                if (notFoundException != null)
                {
                    str = str + notFoundException.FusionLog;
                }

                stringBuilder.Append(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0}{1}: {2}", flag ? (object)string.Empty : (object)" ---> ", (object)exception.GetType(), (object)str));
                flag = false;
            }

            return ((object)stringBuilder).ToString();
        }
    }
}
