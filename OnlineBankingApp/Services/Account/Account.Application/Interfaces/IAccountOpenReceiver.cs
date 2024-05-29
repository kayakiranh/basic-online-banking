using Account.Application.DataTransferObjects;

namespace Account.Application.Interfaces
{
    public interface IAccountOpenReceiver
    {
        void OpenAccount(AccountCreateResponseDto model);
    }
}