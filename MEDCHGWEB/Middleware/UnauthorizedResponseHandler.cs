using Microsoft.AspNetCore.Components;
using System.Net;

namespace MEDCHGWEB.Middleware
{
    public class UnauthorizedResponseHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;

        public UnauthorizedResponseHandler(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("/Account/Logout");
            }

            return response;
        }
    }
}
