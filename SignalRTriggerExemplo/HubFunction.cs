using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.SignalR;

namespace SignalRTriggerExemplo
{
    public class HubFunction : ServerlessHub
    {
        [FunctionName("index")]
        public IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "frontend", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "HubFunction")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("SendMessage")]
        public async Task SendMessage([SignalRTrigger(ConnectionStringSetting = "AzureSignalRConnectionString")] InvocationContext invocationContext,
            string message)
        {
            await Clients.All.SendAsync("newMessage", $"mensagem: {message} ");
        }

        [FunctionName("SendMessageToGroup")]
        public async Task SendMessageToGroup([SignalRTrigger(ConnectionStringSetting = "AzureSignalRConnectionString")] InvocationContext invocationContext,
            string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("newMessage", $"Grupo: {groupName} - Mensagem: {message}");
        }

        [FunctionName("AddToGroup")]
        public async Task AddToGroup([SignalRTrigger(ConnectionStringSetting = "AzureSignalRConnectionString")] InvocationContext invocationContext,
            string groupName)
        {
            await Groups.AddToGroupAsync(invocationContext.ConnectionId, groupName);
        }
    }
}
