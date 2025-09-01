using ChatBot.Data;
using Microsoft.AspNetCore.Identity;

namespace ChatBot.Services
{
    public class NoOpEmailSender : IEmailSender<ApplicationUser>
    {
        //Wszystkie trzy metody są wymagane przez interfejs IEmailSender<T>, ale po prostu nic nie robią.
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
            => Task.CompletedTask;

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
            => Task.CompletedTask;

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
            => Task.CompletedTask;
    }
}
