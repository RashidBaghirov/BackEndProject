using BackEndProject.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Diagnostics.CodeAnalysis;

namespace BackEndProject.Utilities.Comparer
{
	public class ProductComparer : IEqualityComparer<Product>
	{
		public bool Equals(Product? x, Product? y)
		{
			if (Equals(x?.Id, y?.Id)) return true;
			return false;
		}

		public int GetHashCode([DisallowNull] Product obj)
		{
			throw new NotImplementedException();
		}
	}
}
