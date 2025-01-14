using Microsoft.AspNetCore.Identity;

namespace FenixEmail.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string Nome { get; set; }

        public string Sobrenome { get; set; }
    }
}
