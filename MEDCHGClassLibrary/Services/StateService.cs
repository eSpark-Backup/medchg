using MEDCHGClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Services
{
    public class StateService 
    {
        public string MyVariable { get; set; }
        public string AdminVariable { get; set; }
        public string PreviousURL { get; set; }
        // Define the event with a delegate
        public event Action YourEvent;

        public string AccessToken { get; set; }

        public void SetAccessToken(string token)
        {
            AccessToken = token;
        }

        public void ClearAccessToken()
        {
            AccessToken = null;
        }

        // Method to trigger the event
        public void InvokeYourEvent()
        {
            YourEvent?.Invoke();
        }
    }
}
