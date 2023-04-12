using BackEndProject.Entities;
using System.Diagnostics.CodeAnalysis;

namespace BackEndProject.Utilities.Comparer
{
	public class ProductCategoryComparer : IEqualityComparer<ProductCategory>
	{
		public bool Equals(ProductCategory? x, ProductCategory? y)
		{
			if (Equals(x?.Category.Id, y.Category.Id)) return true;
			return false;
		}

		public int GetHashCode([DisallowNull] ProductCategory obj)
		{
			throw new NotImplementedException();
		}
	}
}
