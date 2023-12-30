namespace Neo.Models
{
    public class EventDisplayModel
    {
        public string Id { get; set; }
        public string AuthorPublicKey { get; set; }
        public string CreatedAt { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; }
        public List<string> Tags { get; set; }
    }
}
