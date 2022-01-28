using Hero.Chatbot.Domain.ExtraService;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.UnitOfWork;
using System.Collections.Generic;

namespace Hero.Chatbot.Service
{
    public class ExtraServiceService : IExtraServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExtraServiceRepository _extraServiceRepository;

        public ExtraServiceService(IExtraServiceRepository extraServiceRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _extraServiceRepository = extraServiceRepository;
        }

        public IEnumerable<ExtraService> GetExtraServices()
        {
            using (_unitOfWork.CreateConnection())
            {
                var result = _extraServiceRepository.FindAll();
                return result;
            }
        }
    }
}
