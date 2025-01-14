using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Authentication.Queries;

public class GetUserInfoQuery : IRequest<UserInfoResult>
{
    public string? UserId { get; set; }
}