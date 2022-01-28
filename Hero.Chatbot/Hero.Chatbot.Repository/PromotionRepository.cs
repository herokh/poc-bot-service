using Hero.Chatbot.Domain.Promotion;
using Hero.Chatbot.Repository.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Hero.Chatbot.Repository
{
    public class PromotionRepository : AdoRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(DbProviderFactory dbProviderFactory, IDBContext dbContext, IUnitOfWork unitOfWork)
            : base(dbProviderFactory, dbContext, unitOfWork)
        {
        }

        public override IEnumerable<Promotion> FindAll()
        {
            var sql = @"
                SELECT 
                    promotionId,
                    title,
                    imageUrl,
                    subTitle,
                    linkUrl,
                    linkTitle
                FROM 
                    promotion
            ";

            var result = GetRecords(sql, CommandType.Text);

            return result;
        }

        public override Promotion PopulateRecord(IDataReader reader)
        {
            var model = new Promotion
            {
                PromotionId = (int)reader["promotionId"],
                Title = (string)reader["title"],
                SubTitle = (string)reader["subTitle"],
                ImageUrl = (string)reader["imageUrl"],
                LinkTitle = (string)reader["linkTitle"],
                LinkUrl = (string)reader["linkUrl"]
            };

            return model;
        }
    }
}
