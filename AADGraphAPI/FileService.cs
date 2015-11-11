using models = AADGraphAPI.Models;
using Microsoft.Data.OData;
using Microsoft.OData.Client;
using Microsoft.Office365.SharePoint.FileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AADGraphAPI
{
    public class FileService
    {
        internal async Task<List<models.File>> GetMyFilesAsync()
        {
            var sharePointClient = await AuthenticationHelper.EnsureSharePointClientCreatedAsync("MyFiles");

            List<models.File> returnResults = new List<models.File>();

            try
            {
                var filesResults = await sharePointClient.Files.ExecuteAsync();
                var files = filesResults.CurrentPage;

                foreach (IItem fileItem in files)
                {
                    // The item to add to the result set.
                    models.File modelFile = new models.File(fileItem);
                    returnResults.Add(modelFile);
                }
            }
            catch (Exception e)
            {
                //todo something
                return null;
            }

            return returnResults;
        }
    }
}