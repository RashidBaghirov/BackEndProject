namespace P230_Pronia.ViewModels.Cookies
{
    public class CookiesBasketVM
    {
        public List<CookiesBasketItemVM> CookiesBasketItems { get; set; }
        public decimal TotalPrice { get; set; }

        public CookiesBasketVM()
        {
            CookiesBasketItems = new();
        }
    }
}
