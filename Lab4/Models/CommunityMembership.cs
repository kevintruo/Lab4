namespace Lab4.Models
{
    public class CommunityMembership
    {
        public int StudentId { get; set; }
        public string CommunityId { get; set; }

        //Two references navigation property
        public Community Community { get; set; }
        public Student Student { get; set; }
    }
}