namespace Task2;

using System.Net.Http.Json;
using System.Text.Json;

public static class ApiUtils
{
    public const string URI = "https://qa-internship.avito.com/api/1";

	public static async Task<Item> PostNewItem(Item item)
	{
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(URI);

            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            string? result = "";

            var postResponse = await httpClient.PostAsJsonAsync("/api/1/item", item, jsonOptions);
            var responseData = await postResponse.Content.ReadFromJsonAsync<PostItemResponse>(jsonOptions);

            if (responseData != null)
            {
                result = responseData.Status.Split(' ').LastOrDefault();
            }

            item.Id = result;

            return item;
        }
	}
}