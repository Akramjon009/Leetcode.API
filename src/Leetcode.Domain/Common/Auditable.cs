namespace Leetcode.Domain.Common
{
    public class Auditable<TId> : Base<TId>
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
