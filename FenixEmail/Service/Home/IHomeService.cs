using emaildisparator.Models;
using FenixEmail.Models;

namespace emaildisparator.Service.Home
{
    public interface IHomeService
    {
        Task CreateAsync(RegistrarUserModel registerUserModel);

        Task<List<UsuarioViewModel>> GetAllAsync();

        Task dispararTodosEmailsAsync(List<string> selectedEmails);

        Task DeleteUserAsync(string userId);
    }
}
