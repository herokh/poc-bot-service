using Hero.Shared.Repository;

namespace Hero.Chatbot.Domain.ExtraService
{
    public class ExtraService : IAggregateRoot
    {
        public int ExtraServiceId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string LinkLabel { get; set; }

        public string LinkUrl { get; set; }

        public string ImageUrl { get; set; }
    }
}
