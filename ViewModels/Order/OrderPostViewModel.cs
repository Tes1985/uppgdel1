
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dagnysbageri.ViewModels;

public class OrderPostViewModel
{
    public int CustomerId { get; set; }
    public DateOnly OrderDate { get; set; }

    public IList<OrderItemViewModel> OrderItems { get; set; }
}
