using System;
using Azure.Identity;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount
{
    internal class BlobClientHelper
    {
        public static Azure.Storage.Blobs.BlobContainerClient CreateBlobContainerClient(Uri containerUri)
        {
            //todo: refactor to enable various crednetials...

            return new Azure.Storage.Blobs.BlobContainerClient(containerUri, new DefaultAzureCredential());
        }
    }
}
