using dagnysbageri.Entities;
using dagnysbageri.ViewModels.Address;

namespace dagnysbageri.Interfaces;

public interface IAddressRepository
{
    public Task<Address> AddOne(AddressPostViewModel model);
}