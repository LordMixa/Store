using Dapper.FluentMap;
using Store.Data.Dapper.FluentMapConfigurations.FluentEntityMaps;

namespace Store.Data.Dapper.FluentMapConfigurations
{
    public class FluentMapConfig
    {
        public static void Initialize()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new AuthorMap());
                config.AddMap(new CategoryMap());
            });
        }
    }
}
