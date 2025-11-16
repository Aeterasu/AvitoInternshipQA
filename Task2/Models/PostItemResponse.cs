namespace Task2;

using System.Text.Json.Serialization;

public class PostItemResponse
{
	[JsonPropertyName("status")]
	public string Status { get; private set; } = "";

	public PostItemResponse(string status)
	{
		this.Status = status;
	}
}