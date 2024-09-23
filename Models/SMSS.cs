using SalesCommissionsAPI.Models;
using System.Xml.Linq;

namespace SalesCommissionsAPI.Models
{
    public class SMSS
    {
        public string Salesrep { get; set; }
        public int Year { get; set; }
        public int Fmth { get; set; }
        public int Cmth { get; set; }
        public decimal GPBudget { get; set; }
        public string Site { get; set; }
    }
}