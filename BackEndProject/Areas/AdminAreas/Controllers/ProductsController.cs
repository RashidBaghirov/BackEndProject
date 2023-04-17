using BackEndProject.DAL;
using BackEndProject.Entities;
using BackEndProject.Utilities.Enum;
using BackEndProject.Utilities.Extension;
using BackEndProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Areas.AdminAreas.Controllers
{
	[Area("AdminAreas")]
	[Authorize(Roles = "Admin,Moderator")]

	public class ProductsController : Controller
	{
		private readonly ProductDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductsController(ProductDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}
		public IActionResult Index(int page = 1)
		{
			ViewBag.TotalPage = Math.Ceiling((double)_context.Products.Count() / 5);
			ViewBag.CurrentPage = page;

			IEnumerable<Product> products = _context.Products.Include(p => p.ProductImages)
														.Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
														.Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
														 .AsNoTracking().Skip((page - 1) * 5).Take(5).AsEnumerable();
			return View(products);
		}


		public IActionResult Create()
		{
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();
			return View();

		}

		[HttpPost]
		public async Task<IActionResult> Create(ProductVM newProduct)
		{
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();
			if (!ModelState.IsValid)
			{
				return View();
			}
			if (!newProduct.HoverPhoto.IsValidFile("image/") || !newProduct.MainPhoto.IsValidFile("image/"))
			{
				ModelState.AddModelError(string.Empty, "Please choose image file");
				return View();
			}
			if (!newProduct.HoverPhoto.IsValidLength(1) || !newProduct.MainPhoto.IsValidLength(1))
			{
				ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 1MB");
				return View();
			}
			newProduct.DiscountPrice = newProduct.Price - (newProduct.Price * newProduct.Discount / 100);
			Product product = new()
			{
				Name = newProduct.Name,
				Desc = newProduct.Desc,
				ShortDesc = newProduct.ShortDesc,
				Price = newProduct.Price,
				DiscountPrice = newProduct.DiscountPrice,
				Discount = (decimal)newProduct.Discount,
				SKU = newProduct.SKU,
				BarCode = newProduct.BarCode,
				InstructionsId = newProduct.InstructionId,
				GlobalTabId = newProduct.GlobalTabId,
				CollectionsId = newProduct.CollectionId
			};

			var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");
			foreach (var image in newProduct.Images)
			{
				if (!image.IsValidFile("image/") || !image.IsValidLength(1))
				{
					return View();
				}
				ProductImage productImage = new()
				{
					IsMain = false,
					Path = await image.CreateImage(imagefolderPath, "product-page")
				};
				product.ProductImages.Add(productImage);
			}


			ProductImage main = new()
			{
				IsMain = true,
				Path = await newProduct.MainPhoto.CreateImage(imagefolderPath, "product-page")
			};
			product.ProductImages.Add(main);
			ProductImage hover = new()
			{
				IsMain = null,
				Path = await newProduct.HoverPhoto.CreateImage(imagefolderPath, "product-page")
			};
			product.ProductImages.Add(hover);

			foreach (int id in newProduct.CategoryIds)
			{
				ProductCategory category = new()
				{
					CategoryId = id
				};
				product.ProductCategories.Add(category);
			}
			foreach (int id in newProduct.TagIds)
			{
				ProductTag tag = new()
				{
					TagId = id
				};
				product.ProductTags.Add(tag);
			}


			if (newProduct.ColorsSizesQuantity is null)
			{
				ModelState.AddModelError("", "Please Select Color,Size and Quantity");
				return View();
			}
			else
			{
				string[] colorSizeQuantities = newProduct.ColorsSizesQuantity.Split(',');
				foreach (string colorSizeQuantity in colorSizeQuantities)
				{
					string[] datas = colorSizeQuantity.Split('-');
					ProductSizeColor productSizeColor = new()
					{
						SizeId = int.Parse(datas[0]),
						ColorId = int.Parse(datas[1]),
						Quantity = int.Parse(datas[2])
					};
					if (productSizeColor.Quantity > 0)
					{
						product.InStock = true;
					}
					product.ProductSizeColors.Add(productSizeColor);
				}
			}

			_context.Products.Add(product);
			_context.SaveChanges();
			return RedirectToAction("Index", "Products");
		}

		public IActionResult Edit(int id)
		{
			if (id == 0) return BadRequest();
			ProductVM? model = EditedModel(id);
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();

			if (model is null) return BadRequest();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(int id, ProductVM edited)
		{
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();
			ProductVM? model = EditedModel(id);

			Product? product = await _context.Products.Include(p => p.ProductImages).
				Include(pc => pc.ProductCategories).Include(pt => pt.ProductTags).
					Include(psc => psc.ProductSizeColors).
						ThenInclude(pc => pc.Size).
						  Include(psc => psc.ProductSizeColors).
						ThenInclude(pc => pc.Color).
						Include(p => p.Collections).
					FirstOrDefaultAsync(p => p.Id == id);

			if (product is null) return BadRequest();

			IEnumerable<string> removables = product.ProductImages.Where(p => !edited.ImagesId.Contains(p.Id)).Select(i => i.Path).AsEnumerable();
			var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");

			foreach (string removable in removables)
			{
				string path = Path.Combine(imagefolderPath, "product-page", removable);
				ExtensionMethods.DeleteImage(path);
			}

			if (edited.MainPhoto is not null)
			{
				if (!edited.MainPhoto.IsValidFile("image/"))
				{
					ModelState.AddModelError(string.Empty, "Please choose image file");
					return View();
				}
				if (!edited.MainPhoto.IsValidLength(2))
				{
					ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 2MB");
					return View();
				}
				await AdjustPlantPhoto(true, edited.MainPhoto, product);
			}
			if (edited.HoverPhoto is not null)
			{
				if (!edited.HoverPhoto.IsValidFile("image/"))
				{
					ModelState.AddModelError(string.Empty, "Please choose image file");
					return View();
				}
				if (!edited.HoverPhoto.IsValidLength(2))
				{
					ModelState.AddModelError(string.Empty, "Please choose image which size is maximum 2MB");
					return View();
				}
				await AdjustPlantPhoto(null, edited.HoverPhoto, product);
			}

			product.ProductImages.RemoveAll(p => !edited.ImagesId.Contains(p.Id));
			if (edited.Images is not null)
			{
				foreach (var item in edited.Images)
				{
					if (!item.IsValidFile("image/") || !item.IsValidLength(2))
					{
						TempData["NonSelect"] += item.FileName;
						continue;
					}
					ProductImage productImage = new()
					{
						IsMain = false,
						Path = await item.CreateImage(imagefolderPath, "product-page")
					};
					product.ProductImages.Add(productImage);
				}
			}
			if (edited.CategoryIds != null)
			{
				product.ProductCategories.RemoveAll(pt => !edited.CategoryIds.Contains(pt.CategoryId));
				foreach (int categoryId in edited.CategoryIds)
				{
					Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
					if (category is not null)
					{
						ProductCategory productCategory = new() { Category = category };
						product.ProductCategories.Add(productCategory);
					}
				}
			}
			if (edited.TagIds != null)
			{
				product.ProductTags.RemoveAll(pt => !edited.TagIds.Contains(pt.TagId));
				foreach (int tagid in edited.TagIds)
				{
					Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == tagid);
					if (tag is not null)
					{
						ProductTag productTag = new() { Tag = tag };
						product.ProductTags.Add(productTag);
					}
				}
			}

			if (edited.ColorsSizesQuantity is not null)
			{
				string[] colorSizeQuantities = edited.ColorsSizesQuantity.Split(',');
				foreach (string colorSizeQuantityLoop in colorSizeQuantities)
				{
					string[] datas = colorSizeQuantityLoop.Split('-');
					ProductSizeColor productSizeColor = new()
					{
						SizeId = int.Parse(datas[0]),
						ColorId = int.Parse(datas[1]),
						Quantity = int.Parse(datas[2])
					};
					if (productSizeColor.Quantity <= 0)
					{
						product.InStock = false;
					}
					var existingItem = product.ProductSizeColors.FirstOrDefault(p => p.SizeId == productSizeColor.SizeId && p.ColorId == productSizeColor.ColorId);
					if (existingItem != null)
					{
						existingItem.Quantity = productSizeColor.Quantity;
					}
					else
					{
						product.ProductSizeColors.Add(productSizeColor);
					}
				}
			}
			if (edited.ProductSizeColorDelete is not null)
			{
				string[] plantSizeColorsToDeleteIds = edited.ProductSizeColorDelete.Split(',');
				foreach (string rid in plantSizeColorsToDeleteIds)
				{
					int productSizeColorId = int.Parse(rid);
					var itemToDelete = product.ProductSizeColors.FirstOrDefault(p => p.Id == productSizeColorId);
					if (itemToDelete != null)
					{
						product.ProductSizeColors.Remove(itemToDelete);
					}

				}
			}

			product.Name = edited.Name;
			product.Price = edited.Price;
			product.Desc = edited.Desc;
			product.ShortDesc = edited.ShortDesc;
			product.BarCode = edited.BarCode;
			product.Discount = (decimal)edited.Discount;
			product.DiscountPrice = edited.Price - (edited.Price * edited.Discount / 100);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}


		/// <summary>
		/// Retrieves a PlantVM model with the given ID from the database, populating its properties and collections with data from related entities, and returns it.
		/// </summary>
		/// <param name="id">The ID of the plant to retrieve.</param>
		/// <returns>A PlantVM object populated with data from the database, or null if the plant is not found.</returns>
		private ProductVM? EditedModel(int id)
		{
			ProductVM? model = _context.Products.Include(p => p.ProductCategories).
											   Include(P => P.Instructions)
											   .Include(P => P.GlobalTab)
											   .Include(P => P.Collections)
											.Include(p => p.ProductSizeColors).ThenInclude(psc => psc.Color)
											.Include(p => p.ProductTags)
											.Include(p => p.ProductImages).
											Include(p => p.ProductSizeColors).ThenInclude(pss => pss.Size)
											.Select(p =>
												new ProductVM
												{
													Id = p.Id,
													Name = p.Name,
													SKU = p.SKU,
													Desc = p.Desc,
													ShortDesc = p.ShortDesc,
													Price = p.Price,
													Discount = p.Price,
													BarCode = p.BarCode,
													ProductSizeColors = p.ProductSizeColors,
													InstructionId = p.InstructionsId,
													CollectionId = p.CollectionsId,
													GlobalTabId = p.GlobalTabId,
													DiscountPrice = p.DiscountPrice,
													CategoryIds = p.ProductCategories.Select(pc => pc.CategoryId).ToList(),
													TagIds = p.ProductTags.Select(pc => pc.TagId).ToList(),
													AllImages = p.ProductImages.Select(pi => new ProductImage
													{
														Id = pi.Id,
														Path = pi.Path,
														IsMain = pi.IsMain
													}).ToList()
												})
												.FirstOrDefault(p => p.Id == id);
			return model;
		}

		private async Task AdjustPlantPhoto(bool? ismain, IFormFile image, Product product)
		{
			var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");
			string filepath = Path.Combine(imagefolderPath, "product-page", product.ProductImages.FirstOrDefault(p => p.IsMain == ismain).Path);
			ExtensionMethods.DeleteImage(filepath);
			product.ProductImages.FirstOrDefault(p => p.IsMain == ismain).Path = await image.CreateImage(imagefolderPath, "product-page");
		}




		public IActionResult Details(int id)
		{
			if (id == 0) return BadRequest();
			ProductVM? model = EditedModel(id);
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();
			if (model is null) return BadRequest();
			return View(model);
		}

		public IActionResult Delete(int id)
		{
			if (id == 0) return BadRequest();
			ProductVM? model = EditedModel(id);
			ViewBag.GlobalTabs = _context.GlobalTabs.AsEnumerable();
			ViewBag.Instruction = _context.Instructions.AsEnumerable();
			ViewBag.Collection = _context.Collections.AsEnumerable();
			ViewBag.Categories = _context.Categories.AsEnumerable();
			ViewBag.Tags = _context.Tags.AsEnumerable();
			ViewBag.Sizes = _context.Sizes.AsEnumerable();
			ViewBag.Colors = _context.Colors.AsEnumerable();
			if (model is null) return BadRequest();
			return View(model);
		}

		[HttpPost]
		public IActionResult Delete(int id, ProductVM deleteProduct)
		{
			if (id != deleteProduct.Id) return NotFound();
			Product? product = _context.Products.FirstOrDefault(s => s.Id == id);
			if (product is null) return NotFound();
			IEnumerable<string> removables = product.ProductImages.Where(p => !deleteProduct.ImagesId.Contains(p.Id)).Select(i => i.Path).AsEnumerable();
			var imagefolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "skins", "fashion");

			foreach (string removable in removables)
			{
				string path = Path.Combine(imagefolderPath, "product-page", removable);
				ExtensionMethods.DeleteImage(path);
			}
			_context.Products.Remove(product);
			_context.SaveChanges();
			return RedirectToAction(nameof(Index));
		}
	}


}
