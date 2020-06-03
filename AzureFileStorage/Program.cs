using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AzureFileStorage
{
    class Program
    {
        private static readonly string connection;

        static IConfiguration config = new ConfigurationBuilder()
                                           .AddJsonFile("appsettings.json", true, true)
                                           .Build();
        static Program()
        {
            connection = new Config(config).getValue("fileShareConnectionString");
        }

        static void Main(string[] args)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);

            CloudFileClient cloudFileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.  
            CloudFileShare fileShare = cloudFileClient.GetShareReference("doc2020");

            // Ensure that the share exists.  
            if (fileShare.Exists())
            {
                // Get a reference to the root directory for the share.  
                CloudFileDirectory fileDirectory = fileShare.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.  
                CloudFileDirectory customDirectory = fileDirectory.GetDirectoryReference("storage");

                // Ensure that the directory exists.  
                if (customDirectory.Exists())
                {
                    // Get a reference to the file we created previously.  
                    CloudFile fileInfo = customDirectory.GetFileReference("Log1.txt");

                    // Ensure that the file exists.  
                    if (fileInfo.Exists())
                    {
                        // Write the contents of the file to the console window.  
                        Console.WriteLine(fileInfo.DownloadTextAsync().Result);
                    }
                }

               NewFileCreate();
            }
        }

        public static void NewFileCreate()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connection);

            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("doc2020");
            // Ensure that the share exists.  
            if (share.Exists())
            {
                string policyName = "FileSharePolicy" + DateTime.UtcNow.Ticks;

                SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                };

                FileSharePermissions permissions = share.GetPermissions();

                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                share.SetPermissions(permissions);

                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("storage");

                CloudFile file = sampleDir.GetFileReference("Log2.txt");
                string sasToken = file.GetSharedAccessSignature(null, policyName);
                Uri fileSasUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);

                // Create a new CloudFile object from the SAS, and write some text to the file.  
                CloudFile fileSas = new CloudFile(fileSasUri);
                fileSas.UploadText("This file created by the Console App at Runtime");
                Console.WriteLine(fileSas.DownloadText());
            }
        }
    }

    
}
