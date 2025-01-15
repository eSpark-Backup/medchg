using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MEDCHGWEB.Services
{
    public class SignalRService
    {
        private NavigationManager _navigationManager;
        private HubConnection notificationHub;

        public SignalRService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            notificationHub = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/NotificationHub"))
            .WithAutomaticReconnect()
            .Build();

            notificationHub.Closed += async (error) =>
            {
                Console.WriteLine($"Connection closed due to error: {error}");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await notificationHub.StartAsync();
            };

            notificationHub.Reconnected += (connectionId) =>
            {
                Console.WriteLine($"Reconnected with connection ID: {connectionId}");
                return Task.CompletedTask;
            };

            notificationHub.StartAsync().ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Console.WriteLine($"Connected with connection ID: {notificationHub.ConnectionId}");
                }
                else
                {
                    Console.WriteLine($"Failed to connect: {task.Exception?.GetBaseException()}");
                }
            });
        }

        public async Task SendNotification(List<string> userIds)
        {
            try
            {
                await notificationHub.SendAsync("SendNotification", userIds);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending notification: {e.Message}");
            }
        }
    }
}
