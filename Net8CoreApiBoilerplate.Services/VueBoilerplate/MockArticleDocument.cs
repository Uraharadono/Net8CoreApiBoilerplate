using Net8CoreApiBoilerplate.Infrastructure.DbUtility;
using System;

namespace Net8CoreApiBoilerplate.Services.VueBoilerplate
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
