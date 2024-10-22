using Dapper.FluentMap.Mapping;
using Store.Entities.Entities;

namespace Store.Data.Dapper.FluentMapConfigurations.FluentEntityMaps
{
    public class AuthorMap : EntityMap<Author>
    {
        public AuthorMap()
        {
            Map(u => u.Id).ToColumn("AuthorId");
        }
    }
}
