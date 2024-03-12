namespace Novda.Shared;

public class NovdaMetadata
{
    public string ConnectionId { get; set; }
 
    public Guid AppId { get; set; }
    
    public string HttpMethod { get; set; }
    
    public string RequestPayload { get; set; }

    public string LocalAppUrl { get; set; }
    
    public string RequestedUrl { get; set; }
}
