using System;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerException : Exception
    {
        private readonly DhcpErrors error;
        private readonly string message;

        internal DhcpServerException(string apiFunction, DhcpErrors error, string additionalMessage)
        {
            ApiFunction = apiFunction;
            this.error = error;
            ApiErrorMessage = BuildApiErrorMessage(error);
            message = BuildMessage(apiFunction, additionalMessage, error, ApiErrorMessage);
        }

        internal DhcpServerException(string apiFunction, DhcpErrors error)
            : this(apiFunction, error, null)
        { }

        public string ApiFunction { get; }

        public string ApiError => error.ToString();

        internal DhcpErrors ApiErrorNative => error;
        public uint ApiErrorId => (uint)error;

        public string ApiErrorMessage { get; }

        public override string Message => message;

        private string BuildApiErrorMessage(DhcpErrors error)
        {
            var errorType = typeof(DhcpErrors).GetMember(error.ToString());
            if (errorType.Length != 0)
            {
                var errorAttribute = errorType[0].GetCustomAttributes(typeof(DhcpErrorDescriptionAttribute), false);

                if (errorAttribute.Length != 0)
                    return ((DhcpErrorDescriptionAttribute)errorAttribute[0]).Description;
            }

            return "Unknown Error";
        }

        private string BuildMessage(string apiFunction, string additionalMessage, DhcpErrors error, string apiErrorMessage)
        {
            var builder = new StringBuilder();

            if (apiFunction != null)
                builder.Append("An error occurred calling '").Append(apiFunction).Append("'. ");

            if (additionalMessage != null)
                builder.Append(additionalMessage).Append(". ");

            builder.Append(apiErrorMessage).Append(" [").Append(error.ToString()).Append(' ').Append((uint)error).Append(']');

            return builder.ToString();
        }
    }
}
