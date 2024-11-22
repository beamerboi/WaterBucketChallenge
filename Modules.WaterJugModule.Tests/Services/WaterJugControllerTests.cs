using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Web; // Ensure this matches the namespace of your Program.cs
using System.Collections.Generic;

namespace Modules.WaterJugModule.Tests.Services
{
    public class WaterJugControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WaterJugControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SolveEndpoint_ReturnsSolution_ForValidRequest()
        {
            var validRequest = new
            {
                XCapacity = 3,
                YCapacity = 5,
                ZAmountWanted = 4
            };

            var response = await _client.PostAsJsonAsync("/WaterJug/solve", validRequest);

            response.EnsureSuccessStatusCode(); // Assert HTTP 200 status
            var result = await response.Content.ReadFromJsonAsync<WaterJugResponse>();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Solution);
        }

        [Fact]
        public async Task SolveEndpoint_ReturnsBadRequest_ForImpossibleTarget()
        {
            var impossibleRequest = new
            {
                XCapacity = 2,
                YCapacity = 3,
                ZAmountWanted = 7 // Impossible target
            };

            var response = await _client.PostAsJsonAsync("/WaterJug/solve", impossibleRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Assert HTTP 400 status
            var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.NotNull(result);
            Assert.Contains("No solution possible", result.Error);
        }

        [Fact]
        public async Task SolveEndpoint_ReturnsBadRequest_ForInvalidInput()
        {
            var invalidRequest = new
            {
                XCapacity = -1, // Invalid capacity
                YCapacity = 5,
                ZAmountWanted = 4
            };

            var response = await _client.PostAsJsonAsync("/WaterJug/solve", invalidRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Assert HTTP 400 status
            var result = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.NotNull(result);
            Assert.Contains("Invalid input values", result.Error);
        }

        [Fact]
        public async Task SolveEndpoint_CachesSolution_ForRepeatedRequests()
        {
            var validRequest = new
            {
                XCapacity = 3,
                YCapacity = 5,
                ZAmountWanted = 4
            };

            var response1 = await _client.PostAsJsonAsync("/WaterJug/solve", validRequest);
            response1.EnsureSuccessStatusCode();

            var response2 = await _client.PostAsJsonAsync("/WaterJug/solve", validRequest);
            response2.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            var result1 = await response1.Content.ReadFromJsonAsync<WaterJugResponse>();
            var result2 = await response2.Content.ReadFromJsonAsync<WaterJugResponse>();

            if (result2 != null)
                if (result1 != null)
                    Assert.Equal(result1.Solution.Count, result2.Solution.Count);
        }
    }

    // Strongly Typed Models for API Responses
    public class WaterJugResponse(List<SolutionStep> solution)
    {
        public List<SolutionStep> Solution { get; set; } = solution;
    }

    public class SolutionStep(string action)
    {
        public int BucketX { get; set; }
        public int BucketY { get; set; }
        public string Action { get; set; } = action;
        public string? Status { get; set; }
    }

    public class ErrorResponse
    {
        public required string Error { get; set; }
    }
}
