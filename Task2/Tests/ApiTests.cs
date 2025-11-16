namespace Task2;

using System.Text.Json;
using System.Net;
using System.Net.Http.Json;

using Xunit;
using Xunit.Abstractions;

public class ApiTests
{
	private HttpClient _httpClient = new HttpClient();
	private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true
	};

	private readonly ITestOutputHelper _output;

	public ApiTests(ITestOutputHelper output)
	{
		_output = output;

		_httpClient.BaseAddress = new Uri("https://qa-internship.avito.com/api/1");
	}

	[Fact]
	public async void Test_PostSaveItem()
	{
		// arrange
		var requestBody = new Item(
			sellerId: 555999,
			name: "Test",
			price: 100_000,
			new ItemStatistics(
				likes: 999_999_999,
				viewCount: 100_000,
				contacts: 50
			)
		);

		// act

		var postResponse = await _httpClient.PostAsJsonAsync("/api/1/item", requestBody, _jsonOptions);

		// assert

		Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

		var postResponseData = await postResponse.Content.ReadFromJsonAsync<PostItemResponse>(_jsonOptions);

		Assert.NotNull(postResponseData);
		Assert.False(string.IsNullOrEmpty(postResponseData.Status));

		string? newItemId = postResponseData.Status.Split(' ').LastOrDefault();
		Assert.False(string.IsNullOrEmpty(newItemId));
	}
}