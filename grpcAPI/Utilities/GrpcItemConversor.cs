using grpc.Domain.Models;
using grpcAPI.Protos;
using System.ComponentModel;

namespace grpcAPI.Utilities
{
    public static class GrpcItemConversor
    {
        public static ItemModel grpcItem_To_ItemModel(this Item from)
        {
            return new ItemModel
            {
                Id = from.Id,
                Name = from.Name,
                SectionKey = from.SectionKey
            };
        }
        public static Item ItemModel_To_grpcItem(this ItemModel from)
        {
            return new Item
            {
                Id = from.Id,
                Name = from.Name,
                SectionKey = from.SectionKey,
            };
        }

        public static ItemList ItemList_To_grpcItemlist (this IEnumerable<ItemModel> from)
        {
            var list = new ItemList();
            foreach (var item in from)
            {
                list.List.Add(item.Id, item.Name);
            }
            list.Code = "200";
            list.Message = "Successful";
            return list;
        }

        public static IEnumerable<ItemModel> grpcItemList_To_ItemList (this ItemList from, string sectionkey)
        {
            List<ItemModel> list = new();
            foreach (var item in from.List)
            {
                var itemModel = new ItemModel
                {
                    Id = item.Key,
                    Name = item.Value,
                    SectionKey = sectionkey
                };
                list.Add(itemModel);
            }
            return list;
        }
    }
}
