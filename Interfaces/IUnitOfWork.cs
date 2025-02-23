using dagnysbageri.Interfaces;

namespace dagnysbageri.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    IAddressRepository AddressRepository { get; }

    Task<bool> Complete();
    bool Changes();
}
