namespace PrimusFlex.WebServices.DAL
{
    using PrimusFlex.Data;
    using PrimusFlex.Data.Common;
    using PrimusFlex.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    public class KitchenDataAccess
    {
        protected readonly IDbRepository<Kitchen> kitchens;

        public KitchenDataAccess(ApplicationDbContext context)
        {
            kitchens = new DbRepository<Kitchen>(context);
        }

        public int GetLastKitchenId()
        {
            return this.kitchens.All()
                        .OrderByDescending(k => k.CreatedOn)
                        .Select(k => k.Id)
                        .FirstOrDefault();
        }
    }
}