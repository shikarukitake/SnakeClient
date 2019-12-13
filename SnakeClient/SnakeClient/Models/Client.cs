using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnakeClient.Models;
using RestSharp.Serialization.Json;

namespace SnakeClient
{
    public class Client
    {
        private readonly RestClient _restClient;

        public Client(string uri)
        {
            _restClient = new RestClient(uri);
        }

        public async Task<GameBoard> GetBoardAsync()
        {
            var request = new RestRequest("api/snake/get");
            var response = await _restClient.ExecuteGetTaskAsync<GameBoard>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(response.Content);
            return null;
        }

        public async Task DirectionChange(string Direction)
        {
            var request = new RestRequest("api/snake/ChangeDirection", Method.POST);
            request.AddJsonBody(Direction);
            var response = await _restClient.ExecutePostTaskAsync(request);
        }

        public GameBoard GetBoard()
        {
            GameBoard data;
            var request = new RestRequest("api/snake/get", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _restClient.Execute<GameBoard>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(response.Content);
            else
                data = null;

            return data;
        }

        public async Task StartGame()
        {
            var request = new RestRequest("api/snake/PlayOrStop");
            var response = await _restClient.ExecutePostTaskAsync(request);
            //if response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        }

        public async Task<System.Net.HttpStatusCode> CreateMap(Map paramss, bool GameIsActive)
        {
            RestRequest request;
            if (GameIsActive)
                request = new RestRequest("api/snake/recreatemap", Method.POST);
            else
                request = new RestRequest("api/snake/createmap", Method.POST);
            request.AddJsonBody(paramss);
            var response = await _restClient.ExecutePostTaskAsync(request);
            return response.StatusCode;
        }

        public async Task<System.Net.HttpStatusCode> PlayOrStop(Map paramss)
        {
            var request = new RestRequest("api/snake/createmap", Method.POST);
            request.AddJsonBody(paramss);
            var response = await _restClient.ExecutePostTaskAsync(request);
            return response.StatusCode;
        }
    }
}
