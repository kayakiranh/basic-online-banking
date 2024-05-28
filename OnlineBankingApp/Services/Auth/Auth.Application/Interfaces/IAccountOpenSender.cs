using Auth.Application.DataTransferObjects;

namespace Auth.Application.Interfaces
{
    public interface IAccountOpenSender
    {
        void OpenAccount(RegisterRequestDto model);
    }
}