using System;
using System.Text;

namespace Net8CoreApiBoilerplate.Utility.Extensions
{
    public static class GuidExtensions
    {
        public static string GenerateGuidChain(int limit = 10, bool removeSlashes = false)
        {
            StringBuilder guidChain = new StringBuilder();

            for (int i = 0; i < limit; i++)
            {
                guidChain.Append(Guid.NewGuid());
            }

            if (removeSlashes)
            {
                // guidChain.Replace('-', ''); // empty char doesn't work, imagine that ffs
                guidChain.Replace("-", "");
            }

            return guidChain.ToString();
        }
    }
}
