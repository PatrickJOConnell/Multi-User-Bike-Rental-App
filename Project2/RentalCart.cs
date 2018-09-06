using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    class RentalCart
    {

        public RentalCart(string bikeIDs, string cid, string expectedReturn)
        {
            BIDs = bikeIDs;
            CID = cid;
            ExpectedTime = expectedReturn;
        }
        public string BIDs { get; }
        public string CID { get; }
        public string ExpectedTime { get; }
       
       
    }
}
