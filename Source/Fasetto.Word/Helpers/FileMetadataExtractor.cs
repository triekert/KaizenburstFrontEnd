using System;

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
namespace Fasetto.Word
{


    public static class FileMetadataExtractor
    {
        public static (DateTime CreationDate, string Owner) GetFileMetadata(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var fileInfo = new FileInfo(filePath);

            // 1. Extract Creation Date (UTC or Local)
            var creationDate = fileInfo.CreationTime;

            // 2. Extract NTFS Owner
            var ownerName = "Unknown";
            try
            {
                var security = fileInfo.GetAccessControl(AccessControlSections.Owner);
                var owner = security.GetOwner(typeof(NTAccount));
                ownerName = owner?.Value ?? "Unknown";
            }
            catch (Exception)
            {
                // Owner retrieval can occasionally fail if permissions are restricted
            }

            return (creationDate, ownerName);
        }
    }
}
