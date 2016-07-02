using PrimusFlex.Data;
using PrimusFlex.Data.Common;
using PrimusFlex.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PrimusFlex.WebServices.Controllers
{
    [Authorize]
    public class StorageController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        protected IDbRepository<PictureInfo> picturesInfo;

        public StorageController()
        {
            this.picturesInfo = new DbRepository<PictureInfo>(context);
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/storage
        public void Post(PictureInfo model)
        {
            if (ModelState.IsValid)
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
