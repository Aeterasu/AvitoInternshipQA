namespace Task2;

using System.Text.Json.Serialization;

public class ItemStatistics
{
    [JsonPropertyName("likes")]
    public int Likes { get; private set; } = 0;
    [JsonPropertyName("viewCount")]
    public int ViewCount { get; private set; } = 0;
    [JsonPropertyName("contacts")]
    public int Contacts { get; private set; } = 0;

    public ItemStatistics(){}

    public ItemStatistics(int likes, int viewCount, int contacts)
    {
        this.Likes = likes;
        this.ViewCount = viewCount;
        this.Contacts = contacts;
    }
}