using System.Reflection;
using System.Runtime.Loader;

namespace Leetcode.API.Services
{
    public class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public SimpleUnloadableAssemblyLoadContext()
            : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return default;
        }
    }
}
