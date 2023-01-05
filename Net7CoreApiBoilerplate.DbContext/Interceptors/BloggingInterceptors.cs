using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic;

namespace Net7CoreApiBoilerplate.DbContext.Interceptors
{
    internal static class BloggingInterceptors
    {
        public static IEnumerable<IInterceptor> CreateInterceptors()
        {
            List<IInterceptor> interceptors = new()
            {
                new LoggingInterceptor()
            };

            return interceptors.ToArray();
        }
    }
}
