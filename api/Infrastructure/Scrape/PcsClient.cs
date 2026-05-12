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
            ConfigurePlaywrightPaths();
            EnsureChromiumInstalled();
            _pw = await Playwright.CreateAsync();
            _browser = await _pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            return _browser;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private static void ConfigurePlaywrightPaths()
    {
        var candidates = new[]
        {
            Path.GetDirectoryName(typeof(PcsClient).Assembly.Location),
            AppContext.BaseDirectory,
            Path.GetDirectoryName(Environment.ProcessPath),
        };

        foreach (var dir in candidates)
        {
            if (string.IsNullOrEmpty(dir)) continue;

            var driver = Path.Combine(dir, ".playwright", "node", "win32_x64", "node.exe");
            if (File.Exists(driver))
            {
                Environment.SetEnvironmentVariable("PLAYWRIGHT_DRIVER_PATH", driver);
                break;
            }
        }

        var azureHome = Environment.GetEnvironmentVariable("HOME");
        var onAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"))
                      && !string.IsNullOrEmpty(azureHome);
        if (onAzure)
        {
            var browsers = Path.Combine(azureHome!, "data", "playwright-browsers");
            Directory.CreateDirectory(browsers);
            Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", browsers);
            return;
        }

        foreach (var dir in candidates)
        {
            if (string.IsNullOrEmpty(dir)) continue;

            var browsers = Path.Combine(dir, ".playwright-browsers");
            if (Directory.Exists(browsers))
            {
                Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", browsers);
                break;
            }
        }
    }

    private static void EnsureChromiumInstalled()
    {
        var browsersPath = Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH");
        if (string.IsNullOrEmpty(browsersPath)) return;

        if (Directory.Exists(browsersPath) &&
            Directory.EnumerateDirectories(browsersPath, "chromium*").Any())
        {
            return;
        }

        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
        if (exitCode != 0)
            throw new InvalidOperationException($"playwright install chromium failed with exit code {exitCode}");
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