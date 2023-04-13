namespace BackEndProject.ViewModel.Basket
{
	public class BasketVM
	{
		public List<BasketItemVM> BasketItemVMs { get; set; }

		public decimal TotalPrice { get; set; }

		public BasketVM()
		{
			BasketItemVMs = new();
		}
	}
}
