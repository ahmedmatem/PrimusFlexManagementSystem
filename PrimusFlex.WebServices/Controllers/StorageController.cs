namespace PrimusFlex.WebServices.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.AspNet.Identity;

    using Newtonsoft.Json;

    using Data;
    using Data.Common;
    using Data.Models;
    using Common;
    using Models;
    using Web.Infrastructure.Helpers;

    [Authorize]
    [RoutePrefix("api/Storage")]
    public class StorageController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        protected IDbRepository<Image> image;

        public StorageController()
        {
            this.image = new DbRepository<Image>(context);
        }

        /// <summary>
        /// Retrieve information about the images in the storage
        /// </summary>
        /// <returns></returns>

        // GET api/storage/images
        [Route("images")]
        [HttpGet]
        public HttpResponseMessage GetAllImages()
        {
            CloudStorageAccount storageAccount = StorageHelpers.StorageAccount();
            
            //Create the blob client object.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            CloudBlobContainer container = blobClient.GetContainerReference(Constant.IMAGE_STORAGE_CONTAINER_NAME);
            
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

        /// <summary>
        /// Save information about image in the storage
        /// </summary>
        /// <param name="model">Contains information about the image</param>

        // POST api/storage/saveimage
        [Route("saveimage")]
        [HttpPost]
        public HttpResponseMessage SaveImage(KitchenImageModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Image model is not valid!");
            }

            var newImage = new Image()
            {
                Kitchen = new Kitchen
                {
                    SiteName = model.SiteName,
                    PlotNumber = model.PlotNumber,
                    Model = model.KitchenModel,
                },
                Name = model.ImageName,
                Uri = model.Uri,
                OwnerId = User.Identity.GetUserId()
            };

            this.image.Add(newImage);
            this.image.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
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
