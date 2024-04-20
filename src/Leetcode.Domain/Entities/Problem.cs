using Leetcode.Domain.Common;
using Leetcode.Domain.Enums;

namespace Leetcode.Domain.Entities
{
    public class Problem : Auditable<long>
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Acceptance { get; set; } = 0;
        public string DefaultCode { get; set; }
        public EDifficulty Difficulty { get; set; }

        public List<Submission> Submissions { get; set; }
    }
}
