using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreTagHelperDemo.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NodaTime;

namespace AspNetCoreTagHelperDemo.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new PeriodViewModel();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Demo(PeriodViewModel demoInput)
        {
            if (!ModelState.IsValid)
            {
                var wrongItems = new Dictionary<string, ModelErrorCollection>();

                foreach (var keyValue in ModelState)
                {
                    if (keyValue.Value.Errors.Count > 0)
                    {
                        wrongItems.Add(keyValue.Key, keyValue.Value.Errors);
                    }
                }

                var errorMsg = string.Join("; ", wrongItems.Select(x => $"{x.Key} : {x.Value.Select(y => y.ErrorMessage).Aggregate((i, j) => i + ", " + j)}"));
                _logger.LogInformation("Model binding error={@1}", errorMsg);
            }
            else
            {
                var beginStr = ConvertLocalDateTimeToIso8601(demoInput.Begin, demoInput.TimeZoneId);

                var endStr = demoInput.End.HasValue ? ConvertLocalDateTimeToIso8601(demoInput.End.Value, demoInput.TimeZoneId) : "";

                _logger.LogInformation($"Selected Begin: {{{beginStr}}}, End: {{{endStr}}}");
            }
            return View("Index", demoInput);
        }

        private static string ConvertLocalDateTimeToIso8601(DateTime input, string timeZoneId)
        {
            const string ISO8601format = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

            var dateTimeOffset = DateTime.SpecifyKind(input, DateTimeKind.Unspecified);

            var instant = Instant.FromDateTimeOffset(dateTimeOffset);

            var tz = TimeZoneConverter.TZConvert.WindowsToIana(timeZoneId);
            var timeZone = DateTimeZoneProviders.Tzdb[tz];

            var zonedDateTime = new ZonedDateTime(instant, timeZone);
            var utcDateTime = zonedDateTime.ToDateTimeUtc();

            return utcDateTime.ToString(ISO8601format);
        }
    }
}
