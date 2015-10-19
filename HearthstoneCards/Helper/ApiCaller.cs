using System.Threading.Tasks;
using WPDevToolkit;
using WPDevToolkit.Interfaces;

namespace HearthstoneCards.Helper
{
    internal class ApiCaller : BaseApi, ILocatable
    {
        internal Task<string> GetAllCardsAsync()
        {
            var url = "https://omgvamp-hearthstone-v1.p.mashape.com/cards";
            return ApiGetRequestAsync(url);
        }
    }
}
