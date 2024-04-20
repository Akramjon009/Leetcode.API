using Leetcode.Domain.Common;
using Leetcode.Domain.Enums;

namespace Leetcode.Domain.Entities
{
    public class User : Auditable<Guid>
    {
        //public
        public string UserName { get; set; }
        public string Name { get; set; }
        public EGender? Gender { get; set; }
        public string? Location { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Summary { get; set; }
        public string? Website { get; set; }
        public string? Github { get; set; }
        public string? LinkedIn { get; set; }
        public string? Twitter { get; set; }

        //private
        public string PasswordHash { get; set; }
        //...
        //...
        //...

        public List<Submission> Submissions { get; set; }
    }
}
