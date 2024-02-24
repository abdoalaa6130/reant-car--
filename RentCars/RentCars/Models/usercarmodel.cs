using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentCars.Models
{
    public class usercarmodel
    {
        public string Email { get; set; }
        public string email { get; set; }

        public List<carmodel> carmodels { get; set; }
}
}