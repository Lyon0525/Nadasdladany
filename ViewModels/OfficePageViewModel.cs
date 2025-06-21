using System.Collections.Generic;
using Nadasdladany.Models;

namespace Nadasdladany.ViewModels // Or your web project's Models/ViewModels namespace
{
    public class OfficePageViewModel
    {
        public OfficeInfo OfficeDetails { get; set; }
        public List<OfficeHourEntry> OfficeHours { get; set; }
        public List<Representative> KeyStaffMembers { get; set; } // For Jegyző, other listed staff
    }
}