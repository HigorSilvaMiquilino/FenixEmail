namespace FenixEmail.Service.Email
{
    public interface IEmailHelper
    {
        Task LogEmailAsync(string email, string status, string mensagemErro);
    }
}
