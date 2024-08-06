using System;
using Medical.Data;
using Medical.Service.Implementations.Admin;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Medical.Api.Quartz
{
    public class PrintJob : IJob
    {
        private readonly EmailService _emailService;
        private readonly AppDbContext _context;

        public PrintJob(EmailService emailService, AppDbContext appDbContext)
        {
            _emailService = emailService;
            _context = appDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;
            var oneHourLater = now.AddHours(1);

            var appointments = await _context.Appointments
                .Include(a => a.AppUser)
                .Where(a => a.Date >= now && a.Date <= oneHourLater)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                if (appointment.AppUser != null)
                {
                    var to = appointment.AppUser.Email;
                    var subject = "Upcoming Appointment Reminder";
                    var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f4f4f4; padding: 10px; text-align: center; }}
        .content {{ padding: 20px; }}
        .footer {{ font-size: 0.9em; color: #888; text-align: center; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Appointment Reminder</h2>
        </div>
        <div class='content'>
            <p>Dear {appointment.AppUser.FullName},</p>
            <p>This is a friendly reminder that you have an upcoming appointment scheduled for <strong>{appointment.Date:MMMM d, yyyy 'at' h:mm tt}</strong>.</p>
            <p>Please make sure to arrive at the specified time. If you have any questions or need to reschedule, do not hesitate to contact us.</p>
            <p>Thank you and have a great day!</p>
        </div>
        <div class='footer'>
            <p>Best regards,<br>Elm Hospital</p>
        </div>
    </div>
</body>
</html>";

                    _emailService.Send(to, subject, body);
                }
            }
        }
    }

}

