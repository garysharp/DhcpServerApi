using System;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerException : Exception
    {
        private readonly DhcpErrors error;
        private readonly string additionalMessage;
        private string errorMessage;

        internal DhcpServerException(string apiFunction, DhcpErrors error)
        {
            ApiFunction = apiFunction;
            this.error = error;
        }

        internal DhcpServerException(string apiFunction, DhcpErrors error, string additionalMessage)
            : this(apiFunction, error)
        {
            this.additionalMessage = additionalMessage;
        }

        public string ApiFunction { get; }

        public string ApiError => error.ToString();

        public uint ApiErrorId => (uint)error;

        public string ApiErrorMessage => errorMessage ??= GetApiErrorMessage();

        public override string Message
        {
            get
            {
                var builder = new StringBuilder();

                if (ApiFunction != null)
                    builder.Append("An error occurred calling '").Append(ApiFunction).Append("'. ");

                if (additionalMessage != null)
                    builder.Append(additionalMessage).Append(". ");

                builder.Append(ApiErrorMessage).Append(" [").Append(error.ToString()).Append(' ').Append((uint)error).Append(']');

                return builder.ToString();
            }
        }

        private string GetApiErrorMessage()
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

        public override string ToString() => Message;
    }
}
