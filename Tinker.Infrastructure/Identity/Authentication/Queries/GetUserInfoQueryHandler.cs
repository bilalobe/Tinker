using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Authentication.Queries;

public class GetUserInfoQueryHandler(IUserService userService) : IRequestHandler<GetUserInfoQuery, UserInfoResult>
{
    private readonly IUserService _userService = userService;

    public async Task<UserInfoResult> Handle(GetUserInfoQuery request, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(request.UserId);
        if (user == null) return null;

        return new UserInfoResult
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}