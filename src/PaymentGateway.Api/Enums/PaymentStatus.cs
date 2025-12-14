using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace PaymentGateway.Api.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentStatus
{
    Authorized,
    Declined,
    Rejected
}