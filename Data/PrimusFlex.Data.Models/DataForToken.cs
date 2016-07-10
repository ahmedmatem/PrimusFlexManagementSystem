namespace PrimusFlex.Data.Models
{
    using PrimusFlex.Data.Common.Models;

    public class DataForToken : BaseModel<int>
    {
        public string UserName { get; set; }

        public string  Password { get; set; }
    }
}
