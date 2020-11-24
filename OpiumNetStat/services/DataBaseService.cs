using LiteDB;
using OpiumNetStat.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpiumNetStat.services
{
    public class DataBaseService
    {

        public bool RemoteIpExists(string ip)
        {
            var exist = false;


            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatResult>(DB.CollConnections);
                exist = col.Exists(x => x.RemoteIP.Equals(ip.Trim()));
            }
           
            return exist;
        }


        public NetStatResult GetNetStatRecord(string ip)
        {

            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatResult>(DB.CollConnections);
                return col.Query().Where(x => x.RemoteIP.Equals(ip)).FirstOrDefault();
            }
            
        }


        public NetStatResult Upsert(NetStatResult netStat)
        {

            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatResult>(DB.CollConnections);
                var result = col.Query().Where(x => x.RemoteIP.Equals(netStat.RemoteIP)).FirstOrDefault();

                db.BeginTrans();

                if (result is null)
                {
                    col.Upsert(netStat);

                }else
                {
                    result = netStat;
                    col.Upsert(result);
                }

                col.EnsureIndex(x => x.RemoteIP);
                db.Commit();
            }

            return netStat;
        }


       public  List<NetStatResult> Get24HourDataAsync()
        {
            using (var db = new LiteDatabase(DB.Path))
            {
                var col = db.GetCollection<NetStatResult>(DB.CollConnections);

                var data = col.FindAll().Where(x =>x.LastSeen>DateTime.Now.AddHours(-24)).ToList();

                return data;

            }
        }
    }
}
