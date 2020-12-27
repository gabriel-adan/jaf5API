using Domain;
using Domain.Dtos;
using Domain.RepositoryInterfaces;
using Logic.Contracts;
using SharpArch.Domain.PersistenceSupport;
using System;

namespace Logic.Commons
{
    public class CampsLogic : ICampsLogic
    {
        private readonly ICampsRepository campsRepository;
        private readonly ICustomersRepository customersRepository;
        private readonly IRepository<Account> accountsRepository;

        public CampsLogic(ICampsRepository campsRepository, ICustomersRepository customersRepository, IRepository<Account> accountsRepository)
        {
            this.campsRepository = campsRepository;
            this.accountsRepository = accountsRepository;
        }

        public CampAccountDto Create(int customerId, string name, string street, string number, double longitude, double latitude)
        {
            try
            {
                Customer customer = customersRepository.Get(customerId);
                if (customer == null)
                    throw new ArgumentException("Cliente no registrado");
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Debe ingresar un nombre válido para la cancha");
                if (string.IsNullOrEmpty(street))
                    throw new ArgumentException("Debe ingresar una dirección");

                campsRepository.TransactionManager.BeginTransaction();

                Camp camp = new Camp();
                camp.Name = name;
                camp.Street = street;
                camp.Number = number;
                camp.Longitude = longitude;
                camp.Latitude = latitude;
                camp.IsEnabled = true;
                campsRepository.Save(camp);

                Account account = new Account();
                account.CreatedDate = Helper.GetDateTimeZone();
                account.IsEnabled = true;
                account.Customer = customer;
                account.Camp = camp;

                accountsRepository.Save(account);
                campsRepository.TransactionManager.CommitTransaction();
                CampAccountDto campAccountDto = new CampAccountDto();
                campAccountDto.Id = camp.Id;
                campAccountDto.CreatedDate = account.CreatedDate;
                campAccountDto.IsEnabled = camp.IsEnabled;
                campAccountDto.Name = camp.Name;
                campAccountDto.Street = camp.Street;
                campAccountDto.Number = camp.Number;
                campAccountDto.Longitude = camp.Longitude;
                campAccountDto.Latitude = camp.Latitude;
                return campAccountDto;
            }
            catch
            {
                campsRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }
    }
}
