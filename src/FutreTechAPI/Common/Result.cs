using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutreTechAPI.Common
{
    /// <summary>
    /// Result class to wrapp any response
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
		[JsonProperty("isSuccess")]
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        [JsonProperty("errorMessage")]
        public string Error { get; }

        /// <summary>
        /// Gets the internal error.
        /// </summary>
        /// <value>
        /// The internal error.
        /// </value>
        [JsonProperty("systemErrorMessage")]
        public string InternalError { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is failure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is failure; otherwise, <c>false</c>.
        /// </value>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="error">The error.</param>
        /// <param name="internalError"></param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        protected Result(bool isSuccess, string error, string internalError = "")
        {
            if (isSuccess && (error != string.Empty || internalError != string.Empty))
                throw new InvalidOperationException();

            if (!isSuccess && (error == string.Empty && internalError == string.Empty))
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
            InternalError = internalError;
        }

        /// <summary>
        /// Fails the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="internalError"></param>
        /// <returns></returns>
        public static Result Fail(string message, string internalError = "")
        {
            return new Result(false, message, internalError);
        }

        /// <summary>
        /// Fails the specified message.
        /// </summary>
        /// <typeparam name="T">Type of element that result wrap</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="internalError"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(string message, string internalError = "")
        {
            return new Result<T>(default(T), false, message, internalError);
        }

        /// <summary>
        /// Gets messages from failing result and convert to new one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static Result<T> FailWithResult<T>(Result result)
        {
            return new Result<T>(default(T), false, result.Error, result.InternalError);
        }

        /// <summary>
        /// Gets messages from failing result and convert to new one
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static Result FailWithResult(Result result)
        {
            return new Result(false, result.Error, result.InternalError);
        }

        /// <summary>
        /// Oks this instance.
        /// </summary>
        /// <returns></returns>
        public static Result Ok()
        {
            return new Result(true, string.Empty, string.Empty);
        }

        private static readonly Func<Exception, string> _defaultTryErrorHandler = exc => exc.Message;

        /// <summary>
        /// Wrapp a function into result
        /// </summary>
        /// <param name="action">action to execute</param>
        /// <param name="errorHandler">Error handler</param>
        /// <returns></returns>
        public static Result Try(Action action, Func<Exception, string> errorHandler = null)
        {
            errorHandler = errorHandler ?? _defaultTryErrorHandler;

            try
            {
                action();
                return Ok();
            }
            catch (Exception exc)
            {
                string message = errorHandler(exc);
                return Fail(message);
            }
        }

        /// <summary>
        /// Wrapp function into result
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="func">Function to call</param>
        /// <param name="errorHandler">Error handler</param>
        /// <returns></returns>
        public static Result<T> Try<T>(Func<T> func, Func<Exception, string> errorHandler = null)
        {
            errorHandler = errorHandler ?? _defaultTryErrorHandler;

            try
            {
                return Ok(func());
            }
            catch (Exception exc)
            {
                string message = errorHandler(exc);
                return Fail<T>(message);
            }
        }

        /// <summary>
        /// Wrapp function into result
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="func">Async function to call</param>
        /// <param name="errorHandler">Error handler</param>
        /// <returns></returns>
        public static async Task<Result<T>> Try<T>(Func<Task<T>> func, Func<Exception, string> errorHandler = null)
        {
            errorHandler = errorHandler ?? _defaultTryErrorHandler;

            try
            {
                var result = await func().ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception exc)
            {
                string message = errorHandler(exc);
                return Fail<T>(message);
            }
        }

        /// <summary>
        /// Oks the specified value.
        /// </summary>
        /// <typeparam name="T">Type of element</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Combines the specified results.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }
    }

    /// <summary>
    /// Result to wrapp any generic response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        private readonly T _value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="InvalidOperationException"></exception>
	    [JsonIgnore]
        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        /// <summary>
        /// Gets the data and it will be serialized in JSON.
        /// </summary>
        [JsonProperty]
        private object Data
        {
            get
            {
                if (!IsSuccess)
                    return null;

                return _value;
            }
        }

        /// <summary>
        /// Deconstruct this instance check https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct
        /// </summary>
        public void Deconstruct(out bool isSuccess, out bool isFailure)
        {
            isSuccess = IsSuccess;
            isFailure = IsFailure;
        }

        /// <summary>
        /// Deconstruct this instance check https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct
        /// </summary>
        public void Deconstruct(out bool isSuccess, out bool isFailure, out T value)
        {
            isSuccess = IsSuccess;
            isFailure = IsFailure;
            value = IsSuccess ? Value : default(T);
        }

        /// <summary>
        /// Deconstruct this instance check https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct
        /// </summary>
        public void Deconstruct(out bool isSuccess, out bool isFailure, out T value, out string error)
        {
            isSuccess = IsSuccess;
            isFailure = IsFailure;
            value = IsSuccess ? Value : default(T);
            error = IsFailure ? Error : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="error">The error.</param>
        /// <param name="internalError"></param>
        protected internal Result(T value, bool isSuccess, string error, string internalError = "")
            : base(isSuccess, error, internalError)
        {
            _value = value;
        }
    }
}
