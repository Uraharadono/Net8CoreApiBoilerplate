using Net7CoreApiBoilerplate.Infrastructure.DbUtility;
using System;

namespace Net7CoreApiBoilerplate.Services.VueBoilerplate
{
    public class MockArticleDocument : IEntity
    {
        public long Id { get; set; }
        public string OriginalName { get; set; }
        public string UniqueName { get; set; }
        public string DocumentType { get; set; }
        public DateTime Date { get; set; }
        public long ArticleId { get; set; }
    }
}
