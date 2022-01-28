using Hero.Chatbot.Domain.ExtraService;
using Hero.Chatbot.Repository.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Hero.Chatbot.Repository
{
    public class ExtraServiceRepository : AdoRepository<ExtraService>, IExtraServiceRepository
    {
        public ExtraServiceRepository(DbProviderFactory dbProviderFactory, IDBContext dbContext, IUnitOfWork unitOfWork)
            : base(dbProviderFactory, dbContext, unitOfWork)
        {
        }

        public override IEnumerable<ExtraService> FindAll()
        {
            var sql = @"
                SELECT
                    extraServiceId,
                    title,
                    subTitle,
                    linkLabel,
                    linkUrl,
                    imageUrl
                  FROM 
                    extraService
            ";

            var result = GetRecords(sql, CommandType.Text);

            return result;
        }

        public override ExtraService PopulateRecord(IDataReader reader)
        {
            var model = new ExtraService
            {
                ExtraServiceId = (int)reader["extraServiceId"],
                Title = (string)reader["title"],
                SubTitle = (string)reader["subTitle"],
                LinkLabel = (string)reader["linkLabel"],
                LinkUrl = (string)reader["linkUrl"],
                ImageUrl = (string)reader["imageUrl"]
            };
            return model;
        }
    }
}
