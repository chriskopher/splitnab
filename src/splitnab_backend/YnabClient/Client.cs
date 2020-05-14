using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using YnabClient.Model.User;

namespace YnabClient
{
    public class Client
    {
        private const string Api = "https://api.youneedabudget.com/v1";

        private readonly RestClient _client = new RestClient(Api);

        public Client(string personalAccessToken)
        {
            _client.AddDefaultHeader("Authorization", $"Bearer {personalAccessToken}");
            _client.UseSystemTextJson();
        }

        /// <summary>
        /// Returns authenticated user information.
        /// </summary>
        /// <returns></returns>
        public Task<UserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/user", DataFormat.Json);

            return _client.GetAsync<UserResponse>(req);
        }
    }
}
