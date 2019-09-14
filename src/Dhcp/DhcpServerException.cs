using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Dhcp.Native;

namespace Dhcp
{
    public class DhcpServerException : Exception
    {
        private readonly static Dictionary<DhcpServerNativeErrors, string> errorDescriptions = new Dictionary<DhcpServerNativeErrors, string>();
        private readonly string message;

        public string ApiFunction { get; }

        [Obsolete("Use ApiErrorNative"), EditorBrowsable(EditorBrowsableState.Never)]
        public string ApiError => ApiErrorNative.ToString();

        public DhcpServerNativeErrors ApiErrorNative { get; }
        [Obsolete("Use ApiErrorNative"), EditorBrowsable(EditorBrowsableState.Never)]
        public uint ApiErrorId => (uint)ApiErrorNative;

        public string ApiErrorMessage { get; }

        public override string Message => message;

        internal DhcpServerException(string apiFunction, DhcpServerNativeErrors error, string additionalMessage)
        {
            ApiFunction = apiFunction;
            ApiErrorNative = error;
            ApiErrorMessage = BuildApiErrorMessage(error);
            message = BuildMessage(apiFunction, additionalMessage, error, ApiErrorMessage);
        }

        internal DhcpServerException(string apiFunction, DhcpServerNativeErrors error)
            : this(apiFunction, error, null)
        { }

        private string BuildApiErrorMessage(DhcpServerNativeErrors nativeError)
        {
            // try cache
            if (errorDescriptions.TryGetValue(nativeError, out var description))
                return description;

            // lookup via custom attribute
            var errorType = typeof(DhcpServerNativeErrors).GetMember(nativeError.ToString());
            if (errorType.Length != 0)
            {
                var errorAttribute = errorType[0].GetCustomAttributes(typeof(DhcpServerNativeErrorDescriptionAttribute), false);

                if (errorAttribute.Length != 0)
                {
                    description = ((DhcpServerNativeErrorDescriptionAttribute)errorAttribute[0]).Description;
                }
            }

            // add to cache
            errorDescriptions[nativeError] = description ??= "Unknown Error";

            return description;
        }

        private string BuildMessage(string apiFunction, string additionalMessage, DhcpServerNativeErrors error, string apiErrorMessage)
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
