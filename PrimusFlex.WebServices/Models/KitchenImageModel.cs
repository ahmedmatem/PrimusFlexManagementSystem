namespace PrimusFlex.WebServices.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Data.Models.Types;

    public class KitchenImageModel
    {
        public string UserName { get; set; }

        public string SiteName { get; set; }

        public string PlotNumber { get; set; }

        public KitchenModel KitchenModel { get; set; }

        public string ImageName { get; set; }

        public int ImageNumber { get; set; }

        public string Uri { get; set; }
    }
}
