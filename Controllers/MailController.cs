using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MailServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("send")]
        public void Send([FromBody] IEnumerable<string> recipients)
        {
            if (!recipients.Any())
                return;
            MailMessage mail = new MailMessage();
            var to = "";
            foreach (var recipient in recipients)
            {
                to += recipient + ",";
            }
            to = to.Remove(to.Length - 1);
            mail.To.Add(to);
            mail.From = new MailAddress(_configuration["Mail:Address"], _configuration["Mail:DisplayName"], Encoding.UTF8);
            mail.Subject = _configuration["Mail:Subject"];
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = _configuration["Mail:Body"];
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_configuration["Mail:Address"], _configuration["Mail:Password"]);
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
