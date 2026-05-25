using MvcApp.Models;
using System.Xml.Linq;

namespace MvcApp.Services;


public class NewServices
{
    private readonly HttpClient _httpClient;

    public NewServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<NewsItem>> GetNews(string query)
    {
        var url = $"https://news.google.com/rss/search?q={Uri.EscapeDataString(query)}&hl=en-US&gl=US&ceid=US:en";
        var xmlString = await _httpClient.GetStringAsync(url);

        var doc = XDocument.Parse(xmlString);

        var items = doc.Descendants("item");

        var news = items.Select(item => new NewsItem
        {
           Title = item.Element("title")?.Value,
           Link = item.Element("link")?.Value,
           PubDate = item.Element("pubDate")?.Value,
        }).ToList();

        return news;
    }
}