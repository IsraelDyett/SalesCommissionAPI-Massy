namespace SalesCommissionsAPI.Models
{
    public class ActionLog
    {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string PerformedBy { get; set; }  
        public string Entity { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
