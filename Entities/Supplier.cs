using System.Text.Json.Serialization;

namespace uppgdel1.Entities;

public class Supplier
{
  public int SupplierId { get; set; }
  public string SupplierName { get; set; }
  public string ContactPerson { get; set; }
  public string SupplierAddress { get; set; }
  public string SupplierCity { get; set; }
  public string SupplierZipcode { get; set; }
  public string SupplierPhone { get; set; }
  public string Email { get; set; }
 

  [JsonIgnore]
  public IList<SupplierProduct> SupplierProducts { get; set; }
}
