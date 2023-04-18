using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using P230_Pronia.ViewModels.Cookies;
using System.ComponentModel.DataAnnotations;

namespace BackEndProject.ViewModel
{
	public class CheckOutVM
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string Note { get; set; }

		public List<BasketItemVM> BasketItemVMs { get; set; }
		public decimal TotalPrice { get; set; }
		public OrderStatus OrderStatus { get; set; }


	}
}
