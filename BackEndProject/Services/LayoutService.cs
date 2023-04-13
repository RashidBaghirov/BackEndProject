using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.ViewModel.Basket;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackEndProject.Services
{
	public class LayoutService
	{
		private readonly ProductDbContext _context;

		public LayoutService(ProductDbContext context)
		{
			_context = context;
		}

		public Dictionary<string, string> GetSettings()
		{
			Dictionary<string, string> settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);

			return settings;
		}

	}



}
