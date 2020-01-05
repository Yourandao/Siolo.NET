namespace Siolo.NET.Components.Logstash
{
    public class EventDrop : EventBase
    {
        public string md5 { get; set; }
        public string full_class { get; set; }

        public EventDrop()
        {
            event_type = "drop";
        }

        public EventDrop(string ip, string md5, string full_class)
        {
            event_type = "drop";
            this.ip = ip;
            this.md5 = md5;
            this.full_class = full_class;
        }
    }
}
