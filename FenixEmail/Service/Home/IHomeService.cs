using emaildisparator.Models;
using FenixEmail.Data;
using FenixEmail.Models;

namespace emaildisparator.Service.Home
{
    public interface IHomeService
    {
        Task CreateAsync(RegistrarUserModel registerUserModel);

        Task<List<UsuarioViewModel>> GetAllAsync();

        Task<List<EmailLog>> GetEmailLogsAsync();

        Task dispararTodosEmailsAsync(List<string> selectedEmails);

        Task DeleteUserAsync(string userId);
        Task LogEmailAsync(string email, string status, string mensagemErro);
    }
}
