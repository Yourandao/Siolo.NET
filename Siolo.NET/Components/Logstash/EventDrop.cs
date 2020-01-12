namespace Siolo.NET.Components.Logstash
{
    public class EventDrop : EventBase
    {
        public string Md5 { get; set; }
        public string FullClass { get; set; }

        protected EventDrop() => EventType = "drop";

        public EventDrop(string ip, string md5, string fullClass)
        {
            EventType = "drop";
            this.Ip = ip;
            this.Md5 = md5;
            this.FullClass = fullClass;
        }
    }
}
