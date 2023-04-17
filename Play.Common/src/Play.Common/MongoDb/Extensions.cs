using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDb
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));

            // this is an initialization point for those services can be used anywhere in the application
            services.AddSingleton(serviceProvider =>
            {
                // get an instance of IConfiguration
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddTransient<IRepository<T>>(serviceProvider =>
            {
                // Ask an instance of mongodb database
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}