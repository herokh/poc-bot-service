using System.Globalization;

namespace Hero.Chatbot.Web.Context
{
    public static class ApplicationContext
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("th-TH");

        public static CultureInfo CurrentCultureInfo
        {
            get
            {
                return _cultureInfo;
            }
        }
    }
}
