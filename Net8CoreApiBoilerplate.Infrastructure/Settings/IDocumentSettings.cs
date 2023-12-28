namespace Net8CoreApiBoilerplate.Infrastructure.Settings
{
    // Note: 
    // Add suffix "Folder" to all of the params here, as it's only way to distinguish what is directory.
    // I need this to build folders if they are not present in folder. See: FolderBuilder.BuildFolders() method.
    public interface IDocumentSettings
    {
        public string BaseFolder { get; }
        public string ArticleDocumentsFolder { get; }
        public string WordTemplatesFolder { get; }
    }
}
