namespace PrimusFlex.Data.Models
{
    using PrimusFlex.Data.Common.Models;

    public class Phone : BaseModel<int>
    {
        public string IMEI { get; set; }

        public string OwnerId { get; set; }
    }
}
