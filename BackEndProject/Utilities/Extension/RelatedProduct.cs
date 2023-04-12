using BackEndProject.Entities;
using BackEndProject.Utilities.Comparer;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Utilities.Extension
{
	public static class RelatedProduct
	{
		public static List<Product> Related(IQueryable<Product> queryable, Product product, int id)
		{
			List<Product> relateds = new();

			product.ProductCategories.ForEach(pc =>
			{
				List<Product> relatedByCategory = queryable
					.Include(p => p.ProductImages)
					.Include(p => p.ProductCategories)
						.ThenInclude(pc => pc.Category)
					.Include(p => p.ProductTags)
						.ThenInclude(pt => pt.Tag)
					.Include(c => c.Collections)
					.AsEnumerable()
					.Where(
						p => p.ProductCategories.Contains(pc, new ProductCategoryComparer())
						&& p.Id != id
						&& !relateds.Contains(p, new ProductComparer())
					)
					.ToList();
				relateds.AddRange(relatedByCategory);
			});

			product.ProductTags.ForEach(pt =>
			{
				List<Product> relatedByTag = queryable
					.Include(p => p.ProductImages)
					.Include(p => p.ProductCategories)
						.ThenInclude(pc => pc.Category)
					.Include(p => p.ProductTags)
						.ThenInclude(pt => pt.Tag)
					.Include(c => c.Collections)
					.AsEnumerable()
					.Where(
						p => p.ProductTags.Any(x => x.Tag.Id == pt.Tag.Id)
						&& p.Id != id
						&& !relateds.Contains(p, new ProductComparer())
					)
					.ToList();
				relateds.AddRange(relatedByTag);
			});

			return relateds;
		}
	}
}
