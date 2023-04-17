namespace P230_Pronia.ViewModels.Cookies
{
	public class BasketVM
	{
		public List<BasketItemVM> BasketItems { get; set; }
		public decimal TotalPrice { get; set; }

		public BasketVM()
		{
			BasketItems = new();
		}
	}
}
