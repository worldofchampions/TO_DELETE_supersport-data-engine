using Newtonsoft.Json;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;
using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations.Seed;

namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations
{
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SystemSportDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SystemSportDataContext context)
        {
            var feedConsumerEntities = ReadConsumersFromJson();

            var authFeedConsumers = feedConsumerEntities.Select(LegacyAuthFeedConsumerMapper.MapToModel);

            context.LegacyAuthFeedConsumers.AddOrUpdate(p => p.Name, authFeedConsumers.ToArray());

            SeedSchedulingDashboardUsers.Seed(context);
        }

        public static List<LegacyAuthFeedConsumerEntity> ReadConsumersFromJson()
        {
            const string jsonContent = "[{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'562005D8-5523-46E0-976A-79F564F953B6','Id':0,'MethodAccess':[],'Name':'ITV'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'48B6FD88-57D4-4EA6-AF97-34EEF13DE348','Id':5,'MethodAccess':[],'Name':'News24'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'FE3147AD-9F2F-461D-8316-0A0AF2BC5068','Id':0,'MethodAccess':['MATCHDETAILS','VIDEO','LIVE'],'Name':'SA Rugby'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'A22C8A7F-844C-4E19-97FB-0A63A71BA290','Id':0,'MethodAccess':['MATCHDETAILS','VIDEO','LIVE'],'Name':'iMMedia'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'2376429C-3141-4695-9B67-AF389DD74646','Id':0,'MethodAccess':[],'Name':'Mxit'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'6E8FB0A5-7579-4529-B43B-C5B327D00447','Id':0,'MethodAccess':[],'Name':'DStv'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'60C7469D-2610-4A8D-B667-422EA104BE3F','Id':0,'MethodAccess':[],'Name':'Samsung SmartTV App'}" +
                  ",{'AccessItems':[],'Active':false,'AllowAll':false,'AuthKey':'5A68B01A-E8B4-4721-A615-BF5A6D355B62','Id':11,'MethodAccess':[],'Name':'Handicaps'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'FBEB4372-34D6-4802-9C3A-5010716A5A3C','Id':0,'MethodAccess':[],'Name':'Mweb'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'09044C61-5A3A-47A9-B2B0-E42B02DAC5A0','Id':0,'MethodAccess':[],'Name':'ABSA'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'E7868E2F-0A14-42F8-AB91-43CA81D977CE','Id':0,'MethodAccess':[],'Name':'Internal'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'barclays-premier-league'}],'Active':true,'AllowAll':false,'AuthKey':'6e11a1c9-1bee-4fcf-a01a-fdfb48618c91','Id':15,'MethodAccess':['MATCHDETAILS','MATCHDETAILS','MATCHDETAILS','MATCHDETAILS'],'Name':'testConsumer'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'barclays-premier-league'}],'Active':false,'AllowAll':true,'AuthKey':'ba914268-c4d6-4db1-9e27-50832c7d435a','Id':16,'MethodAccess':[],'Name':'testAutoConsumer1'}" +
                  ",{'AccessItems':[],'Active':false,'AllowAll':false,'AuthKey':'1d4058bf-7b7f-4fbb-a3f5-01690e7d7cf1','Id':17,'MethodAccess':['VIDEO'],'Name':'Team Talk (Zimbabwe)'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'vodacom-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'1424'},{'MethodAccess':null,'Sport':'rugby','Tournament':'1434'},{'MethodAccess':null,'Sport':'rugby','Tournament':'1444'},{'MethodAccess':null,'Sport':'rugby','Tournament':'absa-u-21'},{'MethodAccess':null,'Sport':'rugby','Tournament':'absa-u-19'},{'MethodAccess':null,'Sport':'rugby','Tournament':'2874'},{'MethodAccess':null,'Sport':'rugby','Tournament':'2884'},{'MethodAccess':null,'Sport':'rugby','Tournament':'2994'}],'Active':true,'AllowAll':false,'AuthKey':'bf7ba020-2d88-4f24-bcb6-d49de34e586c','Id':18,'MethodAccess':['VIDEO','VIDEO','VIDEO','VIDEO','VIDEO','VIDEO','VIDEO','VIDEO'],'Name':'WP Rugby'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'barclays-premier-league'},{'MethodAccess':null,'Sport':'football','Tournament':'uefa-champions-league'},{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'}],'Active':true,'AllowAll':true,'AuthKey':'0a5d91ca-6bed-4f79-8efd-076c80f9c7b7','Id':19,'MethodAccess':['MATCHDETAILS','MATCHDETAILS','MATCHDETAILS'],'Name':'Sport Engage'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'nedbank-cup'},{'MethodAccess':null,'Sport':'football','Tournament':'zimbabwe'}],'Active':false,'AllowAll':false,'AuthKey':'cbf07ae8-4422-4d0d-97dd-f5f65adeb749','Id':20,'MethodAccess':['VIDEO','VIDEO','VIDEO','VIDEO'],'Name':'Native Nedbank'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'supersportunited'},{'MethodAccess':null,'Sport':'football','Tournament':'absa-premiership'},{'MethodAccess':null,'Sport':'football','Tournament':'nedbank-cup'},{'MethodAccess':null,'Sport':'football','Tournament':'mtn8'},{'MethodAccess':null,'Sport':'football','Tournament':'telkom-knockout'}],'Active':true,'AllowAll':true,'AuthKey':'cb29359d-ea30-4aca-96d3-c6af331a9440','Id':21,'MethodAccess':['VIDEO','VIDEO'],'Name':'Team Talk (SuperSport United)'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'vodacom-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'},{'MethodAccess':null,'Sport':'rugby','Tournament':'springboks'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup-first-division'},{'MethodAccess':null,'Sport':'rugby','Tournament':'rugby-championship'}],'Active':true,'AllowAll':false,'AuthKey':'9ac3c060-ca62-4ce3-b92c-7a65c9242f7c','Id':23,'MethodAccess':['MATCHDETAILS','MATCHDETAILS'],'Name':'Vodacom'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'e88b7751-086e-4516-832a-a9d6bec59cfd','Id':0,'MethodAccess':[],'Name':'Alston Elliot (SS Production)'}" +
                  ",{'AccessItems':[],'Active':false,'AllowAll':false,'AuthKey':'3c8a556e-553e-4bf8-a525-b83c87a2bfb4','Id':25,'MethodAccess':[],'Name':'ProVantage'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'0eeebbe5-0bc7-44df-b2b1-2321a1e2bfb0','Id':0,'MethodAccess':[],'Name':'WeChat'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'golf','Tournament':'ngc'}],'Active':true,'AllowAll':false,'AuthKey':'1d50e584-5321-4ed5-be9f-4b59d534aac1','Id':27,'MethodAccess':['VIDEO','VIDEO'],'Name':'Sun International (NGC)'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'943c1bdc-2e75-490f-b92c-4785346ec2c3','Id':28,'MethodAccess':[],'Name':'Ventra Media Group'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'vodacom-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'absa-u-19'},{'MethodAccess':null,'Sport':'rugby','Tournament':'absa-u-21'}],'Active':true,'AllowAll':false,'AuthKey':'60232aca-2a22-4f75-86f4-16b4b1745f2a','Id':29,'MethodAccess':['VIDEO','VIDEO'],'Name':'Blue Bulls'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'cricket','Tournament':'domestic-four-day'},{'MethodAccess':null,'Sport':'cricket','Tournament':'domestic-one-day'},{'MethodAccess':null,'Sport':'cricket','Tournament':'domestic-t20'}],'Active':true,'AllowAll':false,'AuthKey':'5423298f-42de-404f-a58e-98d325cd248c','Id':30,'MethodAccess':['VIDEO','VIDEO'],'Name':'Cape Cobras'}" +
                  ",{'AccessItems':[],'Active':false,'AllowAll':false,'AuthKey':'df71bcbe-89da-4096-b322-72f72a630cfe','Id':31,'MethodAccess':['VIDEO','VIDEO','VIDEO'],'Name':'Teamtalk (Winter Olympics)'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'c7c3960c-8739-4acd-a94c-4bbacfb1fcaa','Id':0,'MethodAccess':[],'Name':'SS Internal (Workflow Systems)'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'421c50fc-bce6-4dc2-b6ab-b2a0d94ac549','Id':0,'MethodAccess':[],'Name':'Thumbtribe'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'ad711f0a-82ee-430f-86bf-e1038d098495','Id':0,'MethodAccess':[],'Name':'Conrad'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'82d510a2-08b0-46b8-92e8-13ad3ebfd138','Id':0,'MethodAccess':[],'Name':'BigData'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'cricket','Tournament':'domestic-one-day'}],'Active':true,'AllowAll':false,'AuthKey':'3625dbcb-085d-4b11-bbb8-311ce7907383','Id':36,'MethodAccess':['VIDEO','VIDEO'],'Name':'BackPage Media'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'barclays-premier-league'},{'MethodAccess':null,'Sport':'rugby','Tournament':'springboks'},{'MethodAccess':null,'Sport':'cricket','Tournament':'sa-team'},{'MethodAccess':null,'Sport':'tennis','Tournament':'atp'}],'Active':false,'AllowAll':false,'AuthKey':'377d4dcf-fc66-4f7d-9aad-64fd6b9aba87','Id':37,'MethodAccess':['VIDEO','VIDEO','VIDEO','VIDEO','VIDEO','VIDEO','VIDEO'],'Name':'SportPilot'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'absa-premiership'},{'MethodAccess':null,'Sport':'football','Tournament':'telkom-knockout'}],'Active':true,'AllowAll':false,'AuthKey':'c9f21ad4-7b26-44f9-b0de-6c0cda693bc0','Id':0,'MethodAccess':['VIDEO'],'Name':'Bloem Celtic'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'vodacom-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'under19'},{'MethodAccess':null,'Sport':'rugby','Tournament':'under21'}],'Active':true,'AllowAll':false,'AuthKey':'09e4060d-ef0d-4838-aa3e-62c2006e8443','Id':39,'MethodAccess':['VIDEO','VIDEO'],'Name':'Sharks Rugby'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'football','Tournament':'absa-premiership'}],'Active':false,'AllowAll':false,'AuthKey':'72a2da55-f7e4-484c-a16e-3852937640d7','Id':40,'MethodAccess':['VIDEO','MATCHDETAILS','VIDEO','VIDEO','MATCHDETAILS','VIDEO'],'Name':'Kaizer Chiefs'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'f0fb2de4-132b-4c82-9619-4dce0115f15d','Id':0,'MethodAccess':[],'Name':'Prodea'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':false,'AuthKey':'04cfd50a-bf9f-41fb-9404-ec2e0e8a8d9e','Id':42,'MethodAccess':['VIDEO','VIDEO','LIVE'],'Name':'Netwerk24'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'vodacom-cup'}],'Active':true,'AllowAll':false,'AuthKey':'5f70226d-62f3-494f-91e9-37e44df93ad2','Id':43,'MethodAccess':['MATCHDETAILS','MATCHDETAILS'],'Name':'Pumas'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'91b1b729-bcb3-409a-9a20-6d405fb47ddd','Id':0,'MethodAccess':[],'Name':'Search'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'29a5881b-ee7d-48ee-b18b-fcd3919b489d','Id':45,'MethodAccess':['MATCHDETAILS','MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE'],'Name':'Airtel La Liga'}" +
                  ",{'AccessItems':[{'MethodAccess':null,'Sport':'rugby','Tournament':'super-rugby'},{'MethodAccess':null,'Sport':'rugby','Tournament':'currie-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'six-nations'},{'MethodAccess':null,'Sport':'rugby','Tournament':'sevens'},{'MethodAccess':null,'Sport':'rugby','Tournament':'champions-cup'},{'MethodAccess':null,'Sport':'rugby','Tournament':'france'},{'MethodAccess':null,'Sport':'rugby','Tournament':'england'}],'Active':false,'AllowAll':false,'AuthKey':'d0b9edd6-6306-4922-b7e1-db569bd967dd','Id':46,'MethodAccess':['MATCHDETAILS','LIVE','MATCHDETAILS','LIVE'],'Name':'DStvDM LABS'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'52924a81-d61f-4a90-ab76-a828b3bef64a','Id':47,'MethodAccess':['MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE'],'Name':'PSL'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'1da38866-a7d3-474c-b570-994f0e530572','Id':48,'MethodAccess':['MATCHDETAILS','VIDEO','LIVE','MATCHDETAILS','VIDEO','LIVE'],'Name':'Orlando Pirates'}" +
                  ",{'AccessItems':[],'Active':true,'AllowAll':true,'AuthKey':'575d7ce8-9651-4cdd-80e7-b707c3350c0b','Id':49,'MethodAccess':['MATCHDETAILS','VIDEO','LIVE'],'Name':'PSL New'}]";

            var feedConsumers = JsonConvert.DeserializeObject<List<LegacyAuthFeedConsumerEntity>>(jsonContent);

            return feedConsumers;
        }
    }
}