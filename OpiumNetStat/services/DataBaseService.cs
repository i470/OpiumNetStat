using LiteDB;
using OpiumNetStat.model;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpiumNetStat.services
{
    public class DataBaseService : IDataBaseService
    {
        IEventAggregator ea;

        public DataBaseService(IEventAggregator _ea)
        {
            ea = _ea;
        }

        public bool RemoteIpExists(string ip)
        {
            var exist = false;


            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatItemViewModel>(DB.CollConnections);
                exist = col.Exists(x => x.RemoteIp.Equals(ip.Trim()));
            }

            return exist;
        }


        public NetStatItemViewModel GetNetStatRecord(string ip)
        {

            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatItemViewModel>(DB.CollConnections);
                return col.Query().Where(x => x.RemoteIp.Equals(ip)).FirstOrDefault();
            }

        }


        public NetStatItemViewModel Upsert(NetStatItemViewModel netStat)
        {

            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatItemViewModel>(DB.CollConnections);
                var result = col.Query().Where(x => x.RemoteIp.Equals(netStat.RemoteIp)).FirstOrDefault();

                db.BeginTrans();

                if (result is null)
                {
                    col.Upsert(netStat);

                }
                else
                {
                    result = netStat;
                    col.Upsert(result);
                }

                col.EnsureIndex(x => x.RemoteIp);
                db.Commit();
            }

            return netStat;
        }


    }
}
