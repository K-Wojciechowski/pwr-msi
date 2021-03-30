using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using MimeKit;
using NodaTime;
using pwr_msi.Models;

namespace pwr_msi.Services {
    public class AccountEmailService {
        private readonly AppConfig _appConfig;
        private readonly MsiDbContext _dbContext;
        private readonly IHtmlLocalizer<AccountEmailService> _htmlLocalizer;
        private readonly IStringLocalizer<AccountEmailService> _localizer;

        public AccountEmailService(AppConfig appConfig, MsiDbContext dbContext,
            IStringLocalizer<AccountEmailService> localizer, IHtmlLocalizer<AccountEmailService> htmlLocalizer) {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _localizer = localizer;
            _htmlLocalizer = htmlLocalizer;
        }

        private async Task SendEmail(User user, string subject, string textBody, string htmlBody) {
            var message = new MimeMessage();
            message.From.Add(address: new MailboxAddress(_appConfig.EmailFromName, _appConfig.EmailFromAddress));
            message.To.Add(address: new MailboxAddress(user.FullName, user.Email));
            message.Subject = subject;
            message.Body = new BodyBuilder {TextBody = textBody, HtmlBody = htmlBody}.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_appConfig.SmtpHost, _appConfig.SmtpPort);
            if (_appConfig.SmtpAuthenticate)
                await smtp.AuthenticateAsync(_appConfig.SmtpUsername, _appConfig.SmtpPassword);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(quit: true);
        }

        private string GetLink(VerificationToken token) {
            return _appConfig.ServerAddress.TrimEnd(trimChar: '/') + $"/user/verify/{token.Token}";
        }

        private async Task<VerificationToken> GenerateAndSaveToken(User user, VerificationTokenType tokenType) {
            var tokenStr = new Guid().ToString();
            var token = new VerificationToken {
                Token = tokenStr,
                User = user,
                IsUsed = false,
                ValidUntil = SystemClock.Instance.GetCurrentInstant().Plus(duration: Duration.FromDays(days: 1)),
                TokenType = tokenType,
            };
            await _dbContext.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        }

        public async Task SendVerificationEmail(User user) {
            var token = await GenerateAndSaveToken(user, VerificationTokenType.VERIFY_EMAIL);
            var link = GetLink(token);
            var expiresAt = token.ValidUntil.ToString();
            await SendEmail(
                user,
                subject: _localizer[name: "Verify your MSI Account"].ToString(),
                textBody: _localizer[
                    name:
                    "Hello!\n\nTo use your MSI account, you need to visit this link:\n{0}\n\nThis link will expire on {1}.",
                    link, expiresAt].ToString(),
                htmlBody: _htmlLocalizer[
                    name:
                    "Hello!<br><br>To use your MSI account, you need to visit this link:<br><a href=\"{0}\">{0}</a><br><br>This link will expire on {1}.",
                    link, expiresAt].ToString()
            );
        }

        public async Task SendResetEmail(User user) {
            var token = await GenerateAndSaveToken(user, VerificationTokenType.RESET_PASSWORD);
            var link = GetLink(token);
            var expiresAt = token.ValidUntil.ToString();
            await SendEmail(
                user,
                subject: _localizer[name: "Reset your MSI Account Password"].ToString(),
                textBody: _localizer[
                    name:
                    "Hello!\n\nTo reset the password for your MSI account, you need to visit this link:\n{0}\n\nThis link will expire on {1}.",
                    link, expiresAt].ToString(),
                htmlBody: _htmlLocalizer[
                    name:
                    "Hello!<br><br>To reset the password for your MSI account, you need to visit this link:<br><a href=\"{0}\">{0}</a><br><br>This link will expire on {1}.",
                    link, expiresAt].ToString()
            );
        }
    }
}
