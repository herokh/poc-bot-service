using Hero.Shared.Repository;

namespace Hero.Chatbot.Domain.Promotion
{
    public class Promotion : IAggregateRoot
    {
        public int PromotionId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string SubTitle { get; set; }

        public string LinkUrl { get; set; }

        public string LinkTitle { get; set; }
    }
}
