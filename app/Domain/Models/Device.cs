namespace Domain.Models;

public class Device
{
    public int Id { get; set;}
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Type { get; set; }
    public string OS { get; set; }
    public string OSVersion { get; set; }
    public string Processor { get; set; }
    public int RamGB { get; set; }
    public string Description { get; set; }
}