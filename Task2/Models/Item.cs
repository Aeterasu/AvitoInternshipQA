namespace Task2;

using System.Text.Json.Serialization;

public class Item
{
	[JsonPropertyName("id")]
	public string? Id { get; set; } = "";
	[JsonPropertyName("sellerId")]
	public int SellerId { get; private set; } = 0;
	[JsonPropertyName("name")]
	public string Name { get; private set; } = "";
	[JsonPropertyName("price")]
	public int Price { get; private set; } = 0;
	public ItemStatistics Statistics { get; private set; } = new();
	[JsonPropertyName("createdAt")]
	public string CreatedAt { get; private set; } = "";

	public Item(){}

	[JsonConstructor]
	public Item(string createdAt, string id, int sellerId, string name, int price, ItemStatistics statistics)
	{
		this.CreatedAt = createdAt;
		this.Id = id;
		this.SellerId = sellerId;
		this.Name = name;
		this.Price = price;
		this.Statistics = statistics;
	}

	public Item(int sellerId, string name, int price, ItemStatistics statistics)
	{
		this.SellerId = sellerId;
		this.Name = name;
		this.Price = price;
		this.Statistics = statistics;
	}
}