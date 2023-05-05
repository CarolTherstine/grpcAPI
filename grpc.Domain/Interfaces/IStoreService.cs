using grpc.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grpc.Domain.Interfaces
{
    public interface IStoreService
    {
        public Task<ItemModel> CreateAsync(ItemModel item);
        public Task<ItemModel> UpdateAsync(ItemModel item);
        public Task<ItemModel> DeleteAsync(string id, string sectionKey);
        public Task<ItemModel> GetAsync(string id, string sectionKey);
        public Task<IEnumerable<ItemModel>> GetAsync(string sectionKey);
    }
}
