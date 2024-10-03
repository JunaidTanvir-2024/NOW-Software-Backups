using Effortless.Api.Features.User.Requests;
using Effortless.Api.Features.User.Response;

using RW;

namespace Effortless.Api.Features.User;

public interface IUserHandler
{
    Task<IResultWrapper<GetUserResponse>> GetUserAsync(GetByPhoneNumberRequest request);
    Task<IResultWrapper<IEnumerable<GetUserResponse>>> GetUsersAsync();
}

internal sealed class UserHandler : IUserHandler
{
    private readonly IUserRepository _userRepository;

    public UserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IResultWrapper<IEnumerable<GetUserResponse>>> GetUsersAsync()
    {
        var usersList = await _userRepository.GetAsync();

        if (usersList is null)
        {
            ResultWrapper.Failure<IEnumerable<GetUserResponse>>();
        }
        return ResultWrapper.Success(usersList?.Select(user => new GetUserResponse()
        {
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Email = user?.Email,
            PhoneNumber = user?.PhoneNumber,
            ProfilePhoto = user?.ProfilePhoto,
        }));
    }
    public async Task<IResultWrapper<GetUserResponse>> GetUserAsync(GetByPhoneNumberRequest request)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);

        if (user is null)
        {
            ResultWrapper.Failure<IEnumerable<GetUserResponse>>();
        }
        return ResultWrapper.Success(new GetUserResponse()
        {
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Email = user?.Email,
            PhoneNumber = user?.PhoneNumber,
            ProfilePhoto = user?.ProfilePhoto,
        });
    }
}
