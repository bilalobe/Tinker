using System.ComponentModel.DataAnnotations;
using MediatR;
using Tinker.Shared.Models.Responses;

namespace Tinker.Core.Security.Commands;

public class EnableMfaCommand : IRequest<MfaResult>
{
    public required string UserId { get; set; }

    [Required]
    [RegularExpression("^[A-Z2-7]{16}$")]
    public required string AuthenticatorKey { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6)]
    [RegularExpression("^[0-9]{6}$")]
    public required string VerificationCode { get; set; }

    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
}