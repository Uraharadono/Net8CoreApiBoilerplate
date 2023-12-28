using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic;

namespace Net8CoreApiBoilerplate.DbContext.Interceptors
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
