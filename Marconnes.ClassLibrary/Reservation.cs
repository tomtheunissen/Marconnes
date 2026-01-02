using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marconnes.ClassLibrary
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public string Accomodation { get; set; }
        public string VPId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AmountGuest { get; set; }

        public Reservation(int reservationId, int userId, string accomodation, string vpId, DateTime startDate, DateTime endDate, int amountGuest)
        {
            ReservationId = reservationId;
            UserId = userId;
            Accomodation = accomodation;
            VPId = vpId;
            StartDate = startDate;
            EndDate = endDate;
            AmountGuest = amountGuest;
        }
    }
}
