using Dapper.FluentMap.Mapping;
using Store.Entities.Entities;

namespace Store.Data.Dapper.FluentMapConfigurations.FluentEntityMaps
{
    public class CategoryMap : EntityMap<Category>
    {
        public CategoryMap()
        {
            Map(_ => _.Id).ToColumn("CategoryId");
        }
    }
}
