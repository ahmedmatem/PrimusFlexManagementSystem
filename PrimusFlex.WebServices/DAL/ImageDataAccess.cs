using PrimusFlex.Data;
using PrimusFlex.Data.Common;
using PrimusFlex.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrimusFlex.WebServices.DAL
{
    public class ImageDataAccess
    {
        protected readonly IDbRepository<Image> images;

        public ImageDataAccess(ApplicationDbContext context)
        {
            images = new DbRepository<Image>(context);
        }

        public List<Image> GetImagesByKitchenId(int kitchenId)
        {
            return this.images.All()
                        .Where(i => i.KitchenId == kitchenId)
                        .Select(i => i)
                        .ToList();
        }
    }
}