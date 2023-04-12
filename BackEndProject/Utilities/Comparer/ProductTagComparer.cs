using BackEndProject.Entities;
using System.Diagnostics.CodeAnalysis;

namespace BackEndProject.Utilities.Comparer
{
	public class ProductTagComparer : IEqualityComparer<ProductTag>
	{
		public bool Equals(ProductTag? x, ProductTag? y)
		{
			if (Equals(x?.Tag.Id, y.Tag.Id)) return true;
			return false;
		}

		public int GetHashCode([DisallowNull] ProductTag obj)
		{
			throw new NotImplementedException();
		}
	}
}
