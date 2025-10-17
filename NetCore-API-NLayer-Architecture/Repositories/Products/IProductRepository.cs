using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Products
{
    public interface IProductRepository : IGenericRepository<Products, int>
    {
        public Task<List<Products>> GetTopPriceProductsAsync(int count);
    }
}