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
    using ViewModels;
    using DAL;

    [Authorize]
    [RoutePrefix("api/Storage")]
    public class StorageController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        protected IDbRepository<Image> image;
        protected IDbRepository<Kitchen> kitchen;
        protected IDbRepository<Site> sites;

        public StorageController()
        {
            this.image = new DbRepository<Image>(context);
            this.kitchen = new DbRepository<Kitchen>(context);
            this.sites = new DbRepository<Site>(context);
        }

        // Get api/storage/lastKitchenImages
        [Route("LastKitchenImages")]
        [HttpGet]
        public HttpResponseMessage GetLastKitchenImages()
        {
            var kitchenDataAccess = new KitchenDataAccess(context);
            var lastKitchenId = kitchenDataAccess.GetLastKitchenId();

            var imageDataAccess = new ImageDataAccess(context);
            var lastKitchenImages = imageDataAccess.GetImagesByKitchenId(lastKitchenId);

            List<ImageViewModel> imageUri = new List<ImageViewModel>();
            foreach (var img in lastKitchenImages)
            {
                imageUri.Add(new ImageViewModel
                {
                    Uri = img.Uri,
                    Name = img.Name,
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, imageUri);
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

        // GET: api/storage/lastkitcheninfo
        [Route("lastkitcheninfo")]
        [HttpGet]
        public HttpResponseMessage LastKitchenInfo(string userName)
        {
            var lastKitchenDate = this.kitchen.All()
                                .Where(k => k.UserName == userName)
                                .OrderByDescending(k => k.CreatedOn)
                                .Select(k => k.CreatedOn)
                                .FirstOrDefault<DateTime>();

            var kitchenInfos = this.kitchen.All()
                                .Where(k => k.UserName == userName
                                    && k.CreatedOn == lastKitchenDate)
                                .Select(k => new KitchenImageModel()
                                {
                                    UserName = k.UserName,
                                    SiteName = k.SiteName,
                                    PlotNumber = k.PlotNumber,
                                    KitchenModel = k.Model,
                                    ImageNumber = k.ImageNumber,
                                })
                                .ToList();

            if (kitchenInfos != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, kitchenInfos);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // GET: api/storage/getsitelist
        [Route("GetSiteList")]
        public List<string> GetSiteList()
        {
            return this.sites.All()
                                .Select(s => s.Name)
                                .ToList();
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


            var kitchenExists = this.kitchen.All().FirstOrDefault(k => k.SiteName == model.SiteName && k.PlotNumber == model.PlotNumber);

            Kitchen newKitchen = new Kitchen();
            if (kitchenExists == null)
            {
                newKitchen = new Kitchen
                {
                    UserName = model.UserName,
                    SiteName = model.SiteName,
                    PlotNumber = model.PlotNumber,
                    Model = model.KitchenModel,
                    ImageNumber = 1,
                };

                this.kitchen.Add(newKitchen);
            }
            else
            {
                kitchenExists.ImageNumber++;

                this.kitchen.Update(kitchenExists);
            }

            this.kitchen.Save();

            var newImage = new Image()
            {
                KitchenId = kitchenExists == null ? newKitchen.Id : kitchenExists.Id,
                OwnerId = User.Identity.GetUserId(),
                Name = model.ImageName,
                Uri = Constant.STORAGE_URI + Constant.IMAGE_STORAGE_CONTAINER_NAME + "/" + model.ImageName,
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
