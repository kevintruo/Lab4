using System.Collections.Generic;

namespace Lab4.Models.ViewModels
{
    public class CommunityViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Community> Communities { get; set; }
        public IEnumerable<CommunityMembership> CommunityMemberships { get; set; }
    }
}
