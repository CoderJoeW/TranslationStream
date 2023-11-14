using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationStream
{
    internal class RedisManager
    {
        private static RedisManager? _instance;

        public static RedisManager Instance
        {
            get
            {
                if (_instance == null) _instance = new RedisManager();

                return _instance;
            }
        }

        private ConfigurationOptions _configuration;

        private ConnectionMultiplexer _redis;

        private readonly string _redisHost = Constants.Config.RedisEndpoint;
        private readonly int _redisPort = 6379;

        public ISubscriber Subscriber;

        public RedisManager()
        {
            _configuration = new ConfigurationOptions()
            {
                EndPoints = { { _redisHost, _redisPort } },
                AllowAdmin = true,
                ConnectTimeout = 300,
                IncludeDetailInExceptions = true,
                SyncTimeout = 300
            };

            _redis = ConnectionMultiplexer.Connect(_configuration);

            Subscriber = _redis.GetSubscriber();
        }


    }
}
