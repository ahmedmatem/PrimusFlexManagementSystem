using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PrimusFlex.Data;
using PrimusFlex.Data.Common;
using PrimusFlex.Data.Models;
using PrimusFlex.WebServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PrimusFlex.WebServices.Controllers
{
    [Authorize]
    [RoutePrefix("api/Storage")]
    public class StorageController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        protected IDbRepository<PictureInfo> picturesInfo;

        public StorageController()
        {
            this.picturesInfo = new DbRepository<PictureInfo>(context);
        }

        // GET api/storage/pictureInfo
        [Route("pictureInfo")]
        [HttpGet]
        public HttpResponseMessage GetPicturesInfo()
        {
            //Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=primusflex;AccountKey=1N+U65eUzC1GpNNuJ9JnMBsziPti12Nopj5WDUHGzDVJJFB2UHkC8boSkZ3li97yQ/qAZ22Ub+Mm2Xtw7diKNw==");

            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference(Constant.IMAGE_STORAGE_CONTAINER_NAME);

            // Use the shared access signature (SAS) to perform container operations
            string sas = StorageHelpers.GetContainerSasUri(container);

            //Create a list to store blob URIs returned by a listing operation on the container.
            List<ICloudBlob> blobList = new List<ICloudBlob>();

            foreach (ICloudBlob blob in container.ListBlobs())
            {
                blobList.Add(blob);
            }

            var blobListAsJson = JsonConvert.SerializeObject(blobList);

            //IEnumerable<PictureInfo> picsInfo = this.picturesInfo.All().ToList<PictureInfo>();

            //var picsInfoAsJson = JsonConvert.SerializeObject(picsInfo);

            return Request.CreateResponse(HttpStatusCode.OK, blobListAsJson);
        }

        // GET api/storage/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/storage/pictureinfo
        [Route("pictureInfo")]
        [HttpPost]
        public void SavePictureInfo(PictureInfo model)
        {
            var pictureInfo = new PictureInfo()
            {
                SiteName = model.SiteName,
                PlotNumber = model.PlotNumber,
                PictureName = model.PictureName
            };

            this.picturesInfo.Add(pictureInfo);
            this.picturesInfo.Save();
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
