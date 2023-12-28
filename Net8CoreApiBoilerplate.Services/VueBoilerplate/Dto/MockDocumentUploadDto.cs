namespace Net8CoreApiBoilerplate.Services.VueBoilerplate.Dto
{
    public class MockDocumentUploadDto
    {
        public long ArticleId { get; set; }
        public string DocumentType { get; set; }
        public byte[] FileData { get; set; }
        public string FileName { get; set; } // I need this property as well, since I have only byte array in my "services" project

        // public long CurrentPublisherId { get; set; }
    }
}
