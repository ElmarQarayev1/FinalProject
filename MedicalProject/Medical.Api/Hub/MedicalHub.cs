using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Medical.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Medical.Api
{
    public class MedicalHub : Hub
    {
       
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

        private readonly UserManager<AppUser> _userManager;

        public MedicalHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.GetUserAsync(Context.User);
                if (user != null)
                {
                   
                    ConnectedUsers[Context.UserIdentifier] = Context.ConnectionId;

                  
                    await Clients.All.SendAsync("ShowConnected", user.Id);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.GetUserAsync(Context.User);
                if (user != null)
                {
                  
                    ConnectedUsers.TryRemove(Context.UserIdentifier, out _);

                  
                    await Clients.All.SendAsync("ShowDisconnected", user.Id);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
