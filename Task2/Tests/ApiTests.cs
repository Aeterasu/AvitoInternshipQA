namespace Task2;

using System.Text.Json;
using System.Net;
using System.Net.Http.Json;

using Xunit;
using Xunit.Abstractions;
using System.Text;
using System.Data.Common;

public class ApiTests : IDisposable
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

	public void Dispose()
	{
		_httpClient.Dispose();
	}

	[Fact]
	public async void Test_PostSaveItem()
	{
		// arrange
		var item = new Item(
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

		var response = await _httpClient.PostAsJsonAsync("/api/1/item", item, _jsonOptions);

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

	public async Task<Item> PostNewItem()
	{
		string? result = "";

		var item = new Item(
			sellerId: 555999,
			name: "Test",
			price: 100_000,
			new ItemStatistics(
				likes: 999_999_999,
				viewCount: 100_000,
				contacts: 50
			)
		);

		var postResponse = await _httpClient.PostAsJsonAsync("/api/1/item", item, _jsonOptions);
		var responseData = await postResponse.Content.ReadFromJsonAsync<PostItemResponse>(_jsonOptions);

		if (responseData != null)
		{
			result = responseData.Status.Split(' ').LastOrDefault();
		}

		item.Id = result;

		return item;
	}

	[Fact]
	public async void Test_GetItemById()
	{
		// arrange

		var item = await PostNewItem();
		string? newItemId = item.Id;

		// act

		var response = await _httpClient.GetAsync($"/api/1/item/{newItemId}");
		List<Item>? items = await response.Content.ReadFromJsonAsync<List<Item>>(_jsonOptions);
		Item? itemBody = items?.FirstOrDefault();

		// assert

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		Assert.NotNull(itemBody);
		Assert.Equal(item.Id, itemBody.Id);
		Assert.Equal(item.SellerId, itemBody.SellerId);
		Assert.Equal(item.Name, itemBody.Name);
		Assert.Equal(item.Price, itemBody.Price);
		Assert.Equal(item.Statistics.Likes, itemBody.Statistics.Likes);
		Assert.Equal(item.Statistics.ViewCount, itemBody.Statistics.ViewCount);
		Assert.Equal(item.Statistics.Contacts, itemBody.Statistics.Contacts);
	}

	[Fact]
	public async void Test_GetStatisticByItemId()
	{
		// arrange

		var item = await PostNewItem();
		string? newItemId = item.Id;

		var response = await _httpClient.GetAsync($"/api/1/statistic/{newItemId}");
		List<ItemStatistics>? statistics = await response.Content.ReadFromJsonAsync<List<ItemStatistics>>(_jsonOptions);
		ItemStatistics? statisticsBody = statistics?.FirstOrDefault();

		// assert

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		Assert.NotNull(statisticsBody);
		Assert.Equal(item.Statistics.Likes, statisticsBody.Likes);
		Assert.Equal(item.Statistics.ViewCount, statisticsBody.ViewCount);
		Assert.Equal(item.Statistics.Contacts, statisticsBody.Contacts);
	}

	[Fact]
	public async void Test_GetSellerById()
	{
		// arrange

		var item = await PostNewItem();

		var response = await _httpClient.GetAsync($"/api/1/{item.SellerId}/item");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}