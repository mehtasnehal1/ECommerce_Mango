using Mango.Services.EMailAPI.Message;
using Mango.Services.EMailAPI.Models.Dto;

namespace Mango.Services.EMailAPI.Services
{
    public interface IEmailService
    {
        Task EMailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
