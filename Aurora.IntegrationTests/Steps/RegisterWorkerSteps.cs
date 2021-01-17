using Aurora.Domain.Models;
using Aurora.IntegrationTests.Drivers;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Aurora.IntegrationTests.Steps
{
    [Binding, Scope(Tag = "registerWorker")]
    public class RegisterWorkerSteps
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private CreateWorkerModel _createWorkerModel;
        private WorkerModel _workerModel;
        private int _workerId;

        public RegisterWorkerSteps()
        {
            _server = ServerSetup.Setup();
            _client = _server.CreateClient();
        }

        [Given(@"os dados deste trabalhador:")]
        public void GivenTheWorkersData(Table table) =>
            _createWorkerModel = table.CreateInstance<CreateWorkerModel>();

        [When(@"o registro está feito")]
        public async Task WhenTheRegisterIsDone()
        {
            try
            {
                var data = JsonConvert.SerializeObject(_createWorkerModel);
                var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/users", stringContent);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var result = await response.Content.ReadAsStringAsync();
                _workerId = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        [When(@"eu recupero este trabalhador através do id")]
        public async Task GivenIRecoverTheWorkerById()
        {
            var response = await _client.GetAsync($"api/users/{_workerId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            _workerModel = JsonConvert.DeserializeObject<WorkerModel>(result);
        }

        [Then(@"este trabalhador existe,retornado, através da base de dados")]
        public void ThenTheIdExistsReturnedOnTheDatabase()
        {
            Assert.Equal(_createWorkerModel.Name, _workerModel.Name);
            Assert.Equal(_createWorkerModel.BirthDate, _workerModel.BirthDate);
            Assert.Equal(_createWorkerModel.Nin, _workerModel.Nin);
        }
    }
}
