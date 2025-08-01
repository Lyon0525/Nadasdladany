using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models; // Required for OfficeHourEntry
using Nadasdladany.ViewModels;
using System;
using System.Collections.Generic; // Required for List
using System.Linq;
using System.Text; // Required for StringBuilder
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Nadasdladany.ViewComponents
{
    public class ContactFooterViewComponent : ViewComponent
    {
        private readonly NadasdladanyDbContext _context;
        private readonly IMemoryCache _cache;

        public ContactFooterViewComponent(NadasdladanyDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            const string cacheKey = "ContactFooterData";

            if (!_cache.TryGetValue(cacheKey, out ContactFooterViewModel contactInfo))
            {
                // Fetch the main office details
                var officeInfo = await _context.OfficeInfos.FirstOrDefaultAsync(oi => oi.Id == 1);

                // --- NEW: Fetch the list of office hours ---
                var officeHours = await _context.OfficeHourEntries
                                        .OrderBy(oh => oh.DisplayOrder)
                                        .ThenBy(oh => oh.DayOfWeek)
                                        .ToListAsync();

                // --- NEW: Call a helper method to format the hours into a single string ---
                string formattedOpeningHours = FormatOpeningHoursForFooter(officeHours);

                if (officeInfo != null)
                {
                    contactInfo = new ContactFooterViewModel
                    {
                        Address = officeInfo.Address,
                        PhoneNumber = officeInfo.PhoneNumber,
                        Email = officeInfo.Email,
                        OpeningHours = formattedOpeningHours // Use the dynamically generated string
                    };
                }
                else
                {
                    // Provide safe default values
                    contactInfo = new ContactFooterViewModel
                    {
                        Address = "8145 Nádasdladány, Fő utca 1. (Placeholder)",
                        PhoneNumber = "+36 (22) 123-456",
                        Email = "info@nadasdladany.hu",
                        OpeningHours = formattedOpeningHours // Still use the dynamic string even if office info is missing
                    };
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                _cache.Set(cacheKey, contactInfo, cacheEntryOptions);
            }

            return View(contactInfo);
        }

        /// <summary>
        /// A helper method to format the list of OfficeHourEntry objects into a
        /// short, readable string for the footer.
        /// </summary>
        private string FormatOpeningHoursForFooter(List<OfficeHourEntry> hours)
        {
            if (hours == null || !hours.Any())
            {
                return "Nyitvatartás nincs megadva.";
            }

            var sb = new StringBuilder();
            foreach (var entry in hours)
            {
                string dayAbbreviation = GetHungarianDayAbbreviation(entry.DayOfWeek);
                // FIX: Replace the comma and space with a line break tag
                sb.Append($"{dayAbbreviation}: {entry.TimeDescription}<br />");
            }

            // Return the raw string with the trailing <br />
            // It's safe because it will be the last element in an <li> and won't cause extra space.
            return sb.ToString();
        }

        /// <summary>
        /// Returns the single-letter Hungarian abbreviation for a DayOfWeek.
        /// </summary>
        private string GetHungarianDayAbbreviation(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "H",
                DayOfWeek.Tuesday => "K",
                DayOfWeek.Wednesday => "Sze",
                DayOfWeek.Thursday => "Cs",
                DayOfWeek.Friday => "P",
                DayOfWeek.Saturday => "Szo",
                DayOfWeek.Sunday => "V",
                _ => "?"
            };
        }
    }
}