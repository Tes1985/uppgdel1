using dagnysbageri.ViewModels.Customer;
namespace dagnysbageri.Interfaces;

public interface ICustomerRepository
{
    public Task<bool> UpdatePerson(int id, UpdateContactPersonViewModel model);
    public Task<IList<CustomersViewModel>> GetAll();
    public Task<CustomerViewModel> GetOne(int id);
    public Task<bool> Add(CustomerPostViewModel model);
}