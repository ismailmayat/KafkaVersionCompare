using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KafkaVersionCompare.Pages;

public class KafkaCompare : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public KafkaCompare(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}