using Domain;
using Domain.Dtos;
using Domain.RepositoryInterfaces;
using Logic.Contracts;
using SharpArch.Domain.PersistenceSupport;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Commons
{
    public class CampsLogic : ICampsLogic
    {
        private readonly ICampsRepository campsRepository;
        private readonly ICustomersRepository customersRepository;
        private readonly IRepository<Account> accountsRepository;
        private readonly IFieldsRepository fieldsRepository;

        public CampsLogic(ICampsRepository campsRepository, ICustomersRepository customersRepository, IRepository<Account> accountsRepository, IFieldsRepository fieldsRepository)
        {
            this.campsRepository = campsRepository;
            this.accountsRepository = accountsRepository;
            this.fieldsRepository = fieldsRepository;
        }

        public CampDto Create(int customerId, string name, string street, string number, double longitude, double latitude, IList<string> fieldNames, int fieldCount)
        {
            try
            {
                Customer customer = customersRepository.Get(customerId);
                Helper.ThrowIfNull(customer, "Cliente no registrado");
                Helper.ThrowIfIsNullOrEmpty(name, "Debe ingresar un nombre válido para la cancha");
                Helper.ThrowIfIsNullOrEmpty(street, "Debe ingresar una dirección");
                Helper.ThrowIf(fieldCount == 0, "Debe indicar la cantidad de canhas que tiene el predio");
                Helper.ThrowIfIsNullOrEmpty<string>(fieldNames, "Ocurrió un error al intentar registrar las canchas.");
                int countMax = fieldNames.Count;
                Helper.ThrowIf(fieldCount > countMax, "Por el momento solo es posible registrar hasta [" + countMax + "] canchas.");

                fieldNames = fieldNames.Take(fieldCount).ToList();

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

                int count = fieldNames.Count;
                for (int i = 0; i < count; i++)
                {
                    Field field = new Field();
                    field.Name = fieldNames[i];
                    field.IsEnabled = true;
                    field.Camp = camp;
                    fieldsRepository.Save(field);
                }

                campsRepository.TransactionManager.CommitTransaction();

                CampDto campDto = new CampDto();
                campDto.Id = camp.Id;
                campDto.IsEnabled = camp.IsEnabled;
                campDto.Name = camp.Name;
                campDto.Street = camp.Street;
                campDto.Number = camp.Number;
                campDto.Longitude = camp.Longitude;
                campDto.Latitude = camp.Latitude;
                return campDto;
            }
            catch
            {
                campsRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }

        public IList<FieldDto> GetFields(int campId)
        {
            try
            {
                IList<FieldDto> fieldDtos = new List<FieldDto>();
                IList<Field> fields = fieldsRepository.List(campId);
                foreach (Field field in fields)
                {
                    FieldDto fieldDto = new FieldDto();
                    fieldDto.Id = field.Id;
                    fieldDto.Name = field.Name;
                    fieldDto.IsEnabled = field.IsEnabled;
                    fieldDto.CampId = campId;
                    fieldDtos.Add(fieldDto);
                }
                return fieldDtos;
            }
            catch
            {
                throw;
            }
        }

        public void EditFieldState(int fieldId, int campId, bool isEnabled)
        {
            try
            {
                Field field = fieldsRepository.Get(fieldId);
                Helper.ThrowIfNull(field, "Datos de cancha inválidos");
                Helper.ThrowIf(field.Camp.Id != campId, "Datos de cancha inválidos");
                field.IsEnabled = isEnabled;
                fieldsRepository.SaveOrUpdate(field);
            }
            catch
            {
                throw;
            }
        }
    }
}
