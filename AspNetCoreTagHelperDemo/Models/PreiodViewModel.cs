using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AspNetCoreTagHelperDemo.Models
{
    public class PeriodViewModel
    {
        public IEnumerable<SelectListItem> PresetTimeZoneIds { get; } = new List<SelectListItem>()
        {
            new SelectListItem("Taipei Standard Time","Taipei Standard Time", true),
            new SelectListItem("Tokyo Standard Time", "Tokyo Standard Time"),
            new SelectListItem("New Zealand Standard Time", "New Zealand Standard Time"),
            new SelectListItem("China Daylight Time(HKT)", "China Daylight Time"),
            new SelectListItem("UTC", "UTC"),
        };

        [Required]
        public string TimeZoneId { get; set; }

        public TimeZoneInfo TargetTimeZone
        {
            get => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            set
            {
                if (PresetTimeZoneIds.Select(x => x.Value).Contains(value.Id))
                {
                    TimeZoneId = value.Id;
                }
                throw new ArgumentException("No such preset Time Zone Ids", TimeZoneId);
            }
        }

        [Required]
        public DateTime Begin { get; set; } = DateTime.Now;
        public DateTime? End { get; set; }
    }


}
