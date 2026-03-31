using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.DeviceDtos;

public class CreateDeviceRequestDto
{
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Type { get; set; }
    public string OS { get; set; }
    public string OSVersion { get; set; }
    public string Processor { get; set; }
    public string RamGB { get; set; }
    public string Description { get; set; }
}
