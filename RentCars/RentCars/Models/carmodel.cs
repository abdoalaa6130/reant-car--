using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentCars.Models
{
    public class carmodel
    {
        public int Id { get; set; }
        public string Mark { get; set; }
        public string email { get; set; }

        public string Model { get; set; }
        public string cat { get; set; }
        public Nullable<int> No { get; set; }
        public string adaptation { get; set; }
        public Nullable<int> NoOfDoors { get; set; }
        public string Image { get; set; }
        public Nullable<int> priceForDay { get; set; }
        public string book { get; set; }
    }
}