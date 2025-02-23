using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dagnysbageri.ViewModels;

public class SalesProductPostViewModel
{
    public int Id { get; set; }
    public string ItemNumber { get; set; }
    public string ProductName { get; set; }
    public double EachPrice { get; set; }
    public double Weight { get; set; }
    public int PackAmount { get; set; }
    public DateOnly ManufactureDate { get; set; }
    public DateOnly BestBeforeDate { get; set; }    
    
}