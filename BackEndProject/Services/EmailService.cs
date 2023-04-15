using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace BackEndProject.Services
{
	public interface IEmailService
	{
		void Send(string to, string subject, string html);
	}

	public class EmailService : IEmailService
	{
		private readonly string _emailFromAddress;
		private readonly string _smtpServer;
		private readonly int _smtpPort;
		private readonly string _smtpUsername;
		private readonly string _smtpPassword;

		public EmailService(string emailFromAddress, string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
		{
			_emailFromAddress = emailFromAddress;
			_smtpServer = smtpServer;
			_smtpPort = smtpPort;
			_smtpUsername = smtpUsername;
			_smtpPassword = smtpPassword;
		}

		public void Send(string to, string subject, string html)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("rashid.baghirov@yandex.com"));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;
			email.Body = new TextPart(TextFormat.Html) { Text = html };

			using var smtp = new SmtpClient();
			smtp.Connect("smtp.yandex.com", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("rashid.baghirov@yandex.com", "0006988323Salam");
			smtp.Send(email);
			smtp.Disconnect(true);
		}
	}
}
