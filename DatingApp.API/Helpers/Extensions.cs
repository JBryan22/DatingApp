using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            // the first parameter with this is just extending it, not being used as an actual parameter
            // we are extending the http reponse class with this method. 
            // we are adding the message to the header as well as allowing the message via access control
            // finally we are allowing cross origin responses (any)
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}