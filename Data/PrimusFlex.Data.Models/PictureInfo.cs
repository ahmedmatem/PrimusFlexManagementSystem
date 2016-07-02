namespace PrimusFlex.Data.Models
{
    using PrimusFlex.Data.Common.Models;

    public class PictureInfo : BaseModel<int>
    {
        public string PictureName { get; set; }

        public string SiteName { get; set; }

        public int PlotNumber { get; set; }
    }
}
