using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ClassLibrary
{
    public class Residence
    {
        public int VPId { get; set; }
        public int Price { get; set; }
        public int AmountGuest { get; set; }

        public Residence(int vpId, int price, int amountGuest) 
        {
            VPId = vpId;
            Price = price;
            AmountGuest = amountGuest;
        }
    }
}
