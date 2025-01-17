using emaildisparator.Models;
using emaildisparator.Service.Email;
using FenixEmail.Data;
using FenixEmail.Models;
using FenixEmail.Service.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace emaildisparator.Service.Home
{
    public class HomeService : IHomeService, IEmailHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly EnviarEmail _enviarEmails;

        public HomeService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           ApplicationDbContext context,
                           EnviarEmail enviarEmails)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _enviarEmails = enviarEmails;
        }

        public async Task CreateAsync(RegistrarUserModel registerUserModel)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = registerUserModel.Email, // Set UserName to Email or another unique value
                NormalizedUserName = _userManager.NormalizeName(registerUserModel.Email), // Normalize UserName
                Nome = registerUserModel.Nome,
                Sobrenome = registerUserModel.Sobrenome,
                Email = registerUserModel.Email,
                NormalizedEmail = _userManager.NormalizeEmail(registerUserModel.Email)

            };
  
            var resultado = await _userManager.CreateAsync(applicationUser, registerUserModel.Senha);
            if (resultado.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var resultado = await _userManager.DeleteAsync(user);
                if (!resultado.Succeeded)
                {
                    throw new Exception("Error deleting user");
                }
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public async Task dispararTodosEmailsAsync(List<string> selectedEmails)
        {
            foreach (var selectedEmail in selectedEmails)
            {
                var data = await _userManager.Users.Where(x => x.Email == selectedEmail).ToListAsync();

                if (data == null || !data.Any())
                {
                    await LogEmailAsync(selectedEmail, EmailStatusEnum.Erro.ToString(), "Usuário não encontrado");
                    throw new Exception("User not found");
                }

                foreach (var user in data)
                {
                    if (user == null || string.IsNullOrEmpty(user.Email))
                    {
                        await LogEmailAsync(selectedEmail, EmailStatusEnum.Erro.ToString(), "Usuário não encontrado");
                        continue;
                    }
                    await _enviarEmails.EnviarEmailslAsync(user.Email, user.Nome);
                    await LogEmailAsync(user.Email, EmailStatusEnum.Sucesso.ToString(), "");
                }
            }
        }

        public async Task<List<UsuarioViewModel>> GetAllAsync()
        {
            var data = await _context.Users
                .OrderBy(x => x.Nome)
                .ToListAsync();

            List<UsuarioViewModel> users = new List<UsuarioViewModel>();

            foreach (var item in data)
            {
                users.Add(new UsuarioViewModel
                {
                    Id = item.Id,
                    Nome = item.Nome,
                    Sobrenome = item.Sobrenome,
                    Email = item.Email,
                });
            }
            return users;
        }

        public async Task<List<EmailLog>> GetEmailLogsAsync()
        {
            return await _context.EmailLogs.OrderByDescending(log => log.DataEnvio).ToListAsync();
        }

        public async Task LogEmailAsync(string email, string status, string mensagemErro)
        {
            var log = new EmailLog
            {
                Email = email,
                Status = status,
                DataEnvio = DateTime.Now,
                MensagemErro = mensagemErro
            };

            _context.EmailLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
