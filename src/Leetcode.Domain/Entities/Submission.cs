using Leetcode.Domain.Common;

namespace Leetcode.Domain.Entities
{
    public class Submission : Auditable<Guid>
    {
        public bool IsSuccessed { get; set; }
        public long ProblemId { get; set; }
        public Guid UserId { get; set; }

        public Problem Problem { get; set; }
        public User User { get; set; }
    }
}
