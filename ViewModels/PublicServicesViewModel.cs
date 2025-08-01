using Nadasdladany.Models;
using System.Collections.Generic;

namespace Nadasdladany.ViewModels
{
    public class PublicServicesViewModel
    {
        // For the GROUPED view (when no category is selected)
        public Dictionary<string, List<UsefulLink>> GroupedLinks { get; set; }

        // For the FILTERED view (when a category is selected)
        public IEnumerable<UsefulLink> FilteredLinks { get; set; }

        // For the filter buttons and modals
        public List<string> AllCategories { get; set; }

        // To know which state we are in
        public string CurrentCategory { get; set; }
    }
}