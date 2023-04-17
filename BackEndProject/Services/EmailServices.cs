using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BackEndProject.Services
{
	public class EmailServices
	{
		private readonly string _smtpHost;
		private readonly int _smtpPort;
		private readonly string _fromEmail;
		private readonly string _fromEmailPassword;

		public EmailServices(string smtpHost, int smtpPort, string fromEmail, string fromEmailPassword)
		{
			_smtpHost = smtpHost;
			_smtpPort = smtpPort;
			_fromEmail = fromEmail;
			_fromEmailPassword = fromEmailPassword;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string message)
		{
			using (var client = new SmtpClient(_smtpHost, _smtpPort))
			{
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(_fromEmail, _fromEmailPassword);
				client.EnableSsl = true;

				var mailMessage = new MailMessage
				{
					From = new MailAddress(_fromEmail),
					Subject = subject,
					Body = message,
					IsBodyHtml = true,
				};

				mailMessage.To.Add(new MailAddress(toEmail));

				await client.SendMailAsync(mailMessage);
			}
		}



	}
}
