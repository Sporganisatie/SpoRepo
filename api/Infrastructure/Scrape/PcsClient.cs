using HtmlAgilityPack;
using Microsoft.Playwright;

namespace SpoRE.Infrastructure.Scrape;

internal static class PcsClient
{
    private static IPlaywright? _pw;
    private static IBrowser? _browser;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    private static async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser != null) return _browser;

        await _initLock.WaitAsync();
        try
        {
            if (_browser != null) return _browser;
            var bundled = Path.Combine(AppContext.BaseDirectory, ".playwright-browsers");
            if (Directory.Exists(bundled))
                Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", bundled);
            _pw = await Playwright.CreateAsync();
            _browser = await _pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            return _browser;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public static async Task<HtmlNode> LoadAsync(string url)
    {
        var browser = await GetBrowserAsync();
        var ctx = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
        });
        var page = await ctx.NewPageAsync();
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        var html = await page.ContentAsync();
        await ctx.CloseAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode;
    }
}