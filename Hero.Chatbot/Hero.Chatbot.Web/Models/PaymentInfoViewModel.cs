namespace Hero.Chatbot.Web.Models
{
    public class PaymentInfoViewModel
    {
        public string PaymentMethod { get; set; }

        public string CardHolderName { get; set; }

        public string CardNumber { get; set; }

        public string CardExpireDate { get; set; }

        public string CardCvc { get; set; }
    }
}
