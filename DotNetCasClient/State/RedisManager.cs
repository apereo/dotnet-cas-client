#if NET45
using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.RedisObjects;
using DotNetCasClient.Configuration;
using DotNetCasClient.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCasClient.State
{
    internal class RedisManager
    {
        private static readonly Lazy<RedisManager> lazy = new Lazy<RedisManager>(() => new RedisManager());
        private static readonly Logger securityLogger = new Logger(Category.Security);

        public static RedisManager Instance { get { return lazy.Value; } }

        private Context _context; //Cache connection
        private string _redisKey;

        private RedisManager()
        {
            var conf = CasClientConfiguration.Config;
            _context = new Context(conf.RedisConnectionString);
            _redisKey = conf.RedisKey;
        }

        /// <summary>
        /// Insert ticket into redis hashes
        /// </summary>
        /// <param name="ticket">CAS Auth ticket</param>
        /// <param name="expireDate">Specific DateTime to expire the item from cache</param>
        /// <returns></returns>
        public bool Add(CasAuthenticationTicket ticket)
        {
            
            var hash = _context.Collections.GetRedisDictionary<string, CasAuthenticationTicket>(_redisKey + ":hash");

            if (!hash.ContainsKey(ticket.ServiceTicket))
            {
                hash.Add(ticket.ServiceTicket, ticket);
            }

            return false;

        }
        
        /// <summary>
        /// Retrives cas auth ticket from redis hashes by service ticket 
        /// </summary>
        /// <param name="serviceTicket">cas service ticket</param>
        /// <returns></returns>
        public CasAuthenticationTicket GetAuthTicket(string serviceTicket)
        {
            var hash = _context.Collections.GetRedisDictionary<string, CasAuthenticationTicket>(_redisKey + ":hash");
            return hash[serviceTicket];
        }
        /// <summary>
        /// Remove all tickets from redis hashes by serviceTickets
        /// </summary>
        /// <param name="serviceTickets">List of serviceTickets from remove</param>
        internal void Remove(IEnumerable<string> serviceTickets)
        {
            var hash = _context.Collections.GetRedisDictionary<string, CasAuthenticationTicket>(_redisKey + ":hash");

            foreach(var st in serviceTickets)
            {
                hash.Remove(st);
            }
        }

        /// <summary>
        /// Return all tickets in redis hashes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CasAuthenticationTicket> GetAllTickets()
        {
            var hash = _context.Collections.GetRedisDictionary<string, CasAuthenticationTicket>(_redisKey + ":hash");
            return hash.Values.Select(v=>v);
        }
        
    }
}
#endif