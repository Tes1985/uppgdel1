using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dagnysbageri.ViewModels.Address;

namespace dagnysbageri.ViewModels.Customer;

public class CustomerViewModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string ContactPerson { get; set; }

    public IList<AddressViewModel> Addresses { get; set; }
    public IList<OrderViewModel> Orders { get; set; }
}