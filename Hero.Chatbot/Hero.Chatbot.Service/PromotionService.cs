using Hero.Chatbot.Domain.Promotion;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;

namespace Hero.Chatbot.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPromotionRepository _promotionRepository;

        public PromotionService(IPromotionRepository promotionRepository,
            IUnitOfWork unitOfWork)
        {
            _promotionRepository = promotionRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Promotion> GetPromotions()
        {
            using (_unitOfWork.CreateConnection())
            {
                var result = _promotionRepository.FindAll();
                return result;
            }
        }
    }
}
