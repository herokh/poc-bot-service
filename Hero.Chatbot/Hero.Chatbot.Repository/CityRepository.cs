using Hero.Chatbot.Domain.City;
using Hero.Chatbot.Repository.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Hero.Chatbot.Repository
{
    public class CityRepository : AdoRepository<City>, ICityRepository
    {
        public CityRepository(DbProviderFactory dbProviderFactory,
            IDBContext dbContext,
            IUnitOfWork unitOfWork)
            : base(dbProviderFactory, dbContext, unitOfWork)
        {
        }

        public override IEnumerable<City> FindAll()
        {
            var sql = @"
            SELECT 
                [cityId],
                [cityIataCode],
                [cityName]
            FROM 
                [city]
            ";

            var result = GetRecords(sql, CommandType.Text);
            return result;
        }

        public override City PopulateRecord(IDataReader reader)
        {
            var model = new City
            {
                CityId = (int)reader["cityId"],
                CityIataCode = (string)reader["cityIataCode"],
                CityName = (string)reader["cityName"],
            };

            return model;
        }
    }
}
