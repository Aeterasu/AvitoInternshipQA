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

	private string _itemId = "";

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

		var response = await _httpClient.PostAsJsonAsync("/api/1/item", requestBody, _jsonOptions);

		// assert

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var responseData = await response.Content.ReadFromJsonAsync<PostItemResponse>(_jsonOptions);

		Assert.NotNull(responseData);
		Assert.False(string.IsNullOrEmpty(responseData.Status));

		string? newItemId = responseData.Status.Split(' ').LastOrDefault();
		Assert.False(string.IsNullOrEmpty(newItemId));

		if (newItemId != null)
		{
			_itemId = newItemId;
		}
	}

	[Fact]
	public async void Test_GetItemById()
	{
		var response = await _httpClient.GetAsync($"/api/1/item/d8971d48-bf9d-40f7-b917-bbbc242a0b16");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async void Test_GetStatisticByItemId()
	{
		var response = await _httpClient.GetAsync($"/api/1/statistic/d8971d48-bf9d-40f7-b917-bbbc242a0b16");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async void Test_GetSellerById()
	{
		var response = await _httpClient.GetAsync($"/api/1/555999/item");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}