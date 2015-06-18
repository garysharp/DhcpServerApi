using Dhcp.Native;
using System;
using System.Text;

namespace Dhcp
{
    public class DhcpServerException : Exception
    {
        private DhcpErrors error;
        private string additionalMessage;
        private Lazy<string> errorMessage;

        internal DhcpServerException(string ApiFunction, DhcpErrors Error)
        {
            this.ApiFunction = ApiFunction;
            this.error = Error;

            this.errorMessage = new Lazy<string>(this.GetApiErrorMessage);
        }

        internal DhcpServerException(string ApiFunction, DhcpErrors Error, string AdditionalMessage)
            : this(ApiFunction, Error)
        {
            this.additionalMessage = AdditionalMessage;
        }

        public string ApiFunction { get; private set; }

        public string ApiError
        {
            get
            {
                return this.error.ToString();
            }
        }

        public uint ApiErrorId
        {
            get
            {
                return (uint)this.error;
            }
        }

        public string ApiErrorMessage
        {
            get
            {
                return this.errorMessage.Value;
            }
        }

        public override string Message
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (this.ApiFunction != null)
                {
                    builder.Append("An error occurred calling '").Append(this.ApiFunction).Append("'. ");
                }

                if (this.additionalMessage != null)
                {
                    builder.Append(this.additionalMessage).Append(". ");
                }

                builder.Append(this.errorMessage.Value).Append(" [").Append(this.error.ToString()).Append(' ').Append((uint)this.error).Append(']');

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
                {
                    return ((DhcpErrorDescriptionAttribute)errorAttribute[0]).Description;
                }
            }

            return "Unknown Error";
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
}