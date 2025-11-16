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

		_httpClient.BaseAddress = new Uri(ApiUtils.URI);
	}

	public void Dispose()
	{
		_httpClient.Dispose();
	}

	[Fact]
	public async void Test_PostSaveItem()
	{
		// arrange
		var item = Item.GetTestItem();

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

	[Fact]
	public async void Test_PostSaveInvalidItem()
	{
		// arrange
		var item = new
		{

		};

		// act

		var response = await _httpClient.PostAsJsonAsync("/api/1/item", item, _jsonOptions);

		// assert

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async void Test_GetItemById()
	{
		// arrange

		var item = await ApiUtils.PostNewItem(Item.GetTestItem());
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
	public async void Test_GetInvalidItemById()
	{
		var response = await _httpClient.GetAsync($"/api/1/item/-1");
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async void Test_GetNonExistentItemById()
	{
		var response = await _httpClient.GetAsync($"/api/1/item/aaaa1111-aa11-aa11-aa11-aaaaaaaa1111");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async void Test_GetSellerById()
	{
		var item = await ApiUtils.PostNewItem(Item.GetTestItem());

		var response = await _httpClient.GetAsync($"/api/1/{item.SellerId}/item");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async void Test_GetInvalidSellerById()
	{
		var response = await _httpClient.GetAsync($"/api/1/bad_request/item");
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async void Test_GetStatisticByItemId()
	{
		var item = await ApiUtils.PostNewItem(Item.GetTestItem());
		string? newItemId = item.Id;

		var response = await _httpClient.GetAsync($"/api/1/statistic/{newItemId}");
		List<ItemStatistics>? statistics = await response.Content.ReadFromJsonAsync<List<ItemStatistics>>(_jsonOptions);
		ItemStatistics? statisticsBody = statistics?.FirstOrDefault();

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		Assert.NotNull(statisticsBody);
		Assert.Equal(item.Statistics.Likes, statisticsBody.Likes);
		Assert.Equal(item.Statistics.ViewCount, statisticsBody.ViewCount);
		Assert.Equal(item.Statistics.Contacts, statisticsBody.Contacts);
	}

	[Fact]
	public async void Test_GetStatisticByInvalidItemId()
	{
		var response = await _httpClient.GetAsync($"/api/1/statistic/-1");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async void Test_GetStatisticByNonExistentItemId()
	{
		var response = await _httpClient.GetAsync($"/api/1/statistic/aaaa1111-aa11-aa11-aa11-aaaaaaaa1111");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}