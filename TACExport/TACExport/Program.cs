using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using Ionic.Zlib;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FileMode = System.IO.FileMode;

namespace TACTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ZXXCollectionFile();
            //Diff();

            InitJP();
            //InitCN();
            OutPutFile();

            JsonList();

            Output();

            Console.WriteLine("完成");
            Console.ReadLine();
        }

        private static void OutPutFile()
        {
            using (var sReader = new StreamReader(new FileStream("DataJP/49744fd6", FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                File.AppendAllText("data\\Ability.json", js["Ability"].ToString(Formatting.None));
                File.AppendAllText("data\\AbilityRank.json", js["AbilityRank"].ToString(Formatting.None));
                File.AppendAllText("data\\AI.json", js["AI"].ToString(Formatting.None));
                File.AppendAllText("data\\AppendUnit.json", js["AppendUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\ArenaDefenseResult.json", js["ArenaDefenseResult"].ToString(Formatting.None));
                File.AppendAllText("data\\ArenaRankResult.json", js["ArenaRankResult"].ToString(Formatting.None));
                File.AppendAllText("data\\ArenaWinResult.json", js["ArenaWinResult"].ToString(Formatting.None));
                File.AppendAllText("data\\Artifact.json", js["Artifact"].ToString(Formatting.None));
                File.AppendAllText("data\\ArtifactLvTbl.json", js["ArtifactLvTbl"].ToString(Formatting.None));
                File.AppendAllText("data\\AwakePieceTbl.json", js["AwakePieceTbl"].ToString(Formatting.None));
                File.AppendAllText("data\\Award.json", js["Award"].ToString(Formatting.None));
                File.AppendAllText("data\\Banner.json", js["Banner"].ToString(Formatting.None));
                File.AppendAllText("data\\BreakObj.json", js["BreakObj"].ToString(Formatting.None));
                File.AppendAllText("data\\Buff.json", js["Buff"].ToString(Formatting.None));
                File.AppendAllText("data\\Challenge.json", js["Challenge"].ToString(Formatting.None));
                File.AppendAllText("data\\ChallengeCategory.json", js["ChallengeCategory"].ToString(Formatting.None));
                File.AppendAllText("data\\CollaboSkill.json", js["CollaboSkill"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCard.json", js["ConceptCard"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardConditions.json", js["ConceptCardConditions"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl1.json", js["ConceptCardLvTbl1"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl2.json", js["ConceptCardLvTbl2"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl3.json", js["ConceptCardLvTbl3"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl4.json", js["ConceptCardLvTbl4"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl5.json", js["ConceptCardLvTbl5"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardLvTbl6.json", js["ConceptCardLvTbl6"].ToString(Formatting.None));
                File.AppendAllText("data\\ConceptCardTrustReward.json", js["ConceptCardTrustReward"].ToString(Formatting.None));
                File.AppendAllText("data\\Cond.json", js["Cond"].ToString(Formatting.None));
                File.AppendAllText("data\\ConvertUnitPieceExclude.json", js["ConvertUnitPieceExclude"].ToString(Formatting.None));
                File.AppendAllText("data\\CustomTarget.json", js["CustomTarget"].ToString(Formatting.None));
                File.AppendAllText("data\\DynamicTransformUnit.json", js["DynamicTransformUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\Evaluation.json", js["Evaluation"].ToString(Formatting.None));
                File.AppendAllText("data\\EventShopUnlock.json", js["EventShopUnlock"].ToString(Formatting.None));
                File.AppendAllText("data\\Fix.json", js["Fix"].ToString(Formatting.None));
                File.AppendAllText("data\\FriendPresentItem.json", js["FriendPresentItem"].ToString(Formatting.None));
                File.AppendAllText("data\\Geo.json", js["Geo"].ToString(Formatting.None));
                File.AppendAllText("data\\Grow.json", js["Grow"].ToString(Formatting.None));
                File.AppendAllText("data\\GuildEmblem.json", js["GuildEmblem"].ToString(Formatting.None));
                File.AppendAllText("data\\GuildFacility.json", js["GuildFacility"].ToString(Formatting.None));
                File.AppendAllText("data\\GuildFacilityLvTbl.json", js["GuildFacilityLvTbl"].ToString(Formatting.None));
                //File.AppendAllText("data\\IconMap.json", js["IconMap"].ToString(Formatting.None));
                File.AppendAllText("data\\InitItem.json", js["InitItem"].ToString(Formatting.None));
                File.AppendAllText("data\\InitPlayer.json", js["InitPlayer"].ToString(Formatting.None));
                File.AppendAllText("data\\InitUnit.json", js["InitUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\Item.json", js["Item"].ToString(Formatting.None));
                File.AppendAllText("data\\Job.json", js["Job"].ToString(Formatting.None));
                File.AppendAllText("data\\JobGroup.json", js["JobGroup"].ToString(Formatting.None));
                File.AppendAllText("data\\JobSet.json", js["JobSet"].ToString(Formatting.None));
                File.AppendAllText("data\\LocalNotification.json", js["LocalNotification"].ToString(Formatting.None));
                //File.AppendAllText("data\\LocCard.json", js["LocCard"].ToString(Formatting.None));
                //File.AppendAllText("data\\LocGear.json", js["LocGear"].ToString(Formatting.None));
                //File.AppendAllText("data\\LocItem.json", js["LocItem"].ToString(Formatting.None));
                //File.AppendAllText("data\\LocUnit.json", js["LocUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\LoginInfo.json", js["LoginInfo"].ToString(Formatting.None));
                File.AppendAllText("data\\Mov.json", js["Mov"].ToString(Formatting.None));
                File.AppendAllText("data\\MultilimitUnitLv.json", js["MultilimitUnitLv"].ToString(Formatting.None));
                File.AppendAllText("data\\Player.json", js["Player"].ToString(Formatting.None));
                File.AppendAllText("data\\PlayerLvTbl.json", js["PlayerLvTbl"].ToString(Formatting.None));
                File.AppendAllText("data\\Premium.json", js["Premium"].ToString(Formatting.None));
                File.AppendAllText("data\\QuestClearUnlockUnitData.json", js["QuestClearUnlockUnitData"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidAreaClearReward.json", js["RaidAreaClearReward"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidBattleReward.json", js["RaidBattleReward"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidBeatReward.json", js["RaidBeatReward"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidBoss.json", js["RaidBoss"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidCompleteReward.json", js["RaidCompleteReward"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidDamageRatioReward.json", js["RaidDamageRatioReward"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidPeriod.json", js["RaidPeriod"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidRescueLevelRange.json", js["RaidRescueLevelRange"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidReward.json", js["RaidReward"].ToString(Formatting.None));
                File.AppendAllText("data\\Rarity.json", js["Rarity"].ToString(Formatting.None));
                File.AppendAllText("data\\Recipe.json", js["Recipe"].ToString(Formatting.None));
                File.AppendAllText("data\\RecommendedArtifact.json", js["RecommendedArtifact"].ToString(Formatting.None));
                File.AppendAllText("data\\Shop.json", js["Shop"].ToString(Formatting.None));
                File.AppendAllText("data\\Skill.json", js["Skill"].ToString(Formatting.None));
                File.AppendAllText("data\\SkillAbilityDerive.json", js["SkillAbilityDerive"].ToString(Formatting.None));
                File.AppendAllText("data\\SkillMotion.json", js["SkillMotion"].ToString(Formatting.None));
                File.AppendAllText("data\\StatusCoefficient.json", js["StatusCoefficient"].ToString(Formatting.None));
                File.AppendAllText("data\\Tips.json", js["Tips"].ToString(Formatting.None));
                File.AppendAllText("data\\Tobira.json", js["Tobira"].ToString(Formatting.None));
                File.AppendAllText("data\\TobiraCategories.json", js["TobiraCategories"].ToString(Formatting.None));
                File.AppendAllText("data\\TobiraConds.json", js["TobiraConds"].ToString(Formatting.None));
                File.AppendAllText("data\\TobiraCondsUnit.json", js["TobiraCondsUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\TobiraRecipe.json", js["TobiraRecipe"].ToString(Formatting.None));
                File.AppendAllText("data\\TowerRank.json", js["TowerRank"].ToString(Formatting.None));
                File.AppendAllText("data\\TowerScore.json", js["TowerScore"].ToString(Formatting.None));
                File.AppendAllText("data\\Trick.json", js["Trick"].ToString(Formatting.None));
                File.AppendAllText("data\\Trophy.json", js["Trophy"].ToString(Formatting.None));
                File.AppendAllText("data\\TrophyCategory.json", js["TrophyCategory"].ToString(Formatting.None));
                File.AppendAllText("data\\Unit.json", js["Unit"].ToString(Formatting.None));
                File.AppendAllText("data\\UnitGroup.json", js["UnitGroup"].ToString(Formatting.None));
                File.AppendAllText("data\\UnitJobOverwrite.json", js["UnitJobOverwrite"].ToString(Formatting.None));
                File.AppendAllText("data\\UnitLvTbl.json", js["UnitLvTbl"].ToString(Formatting.None));
                File.AppendAllText("data\\UnitUnlockTime.json", js["UnitUnlockTime"].ToString(Formatting.None));
                File.AppendAllText("data\\Unlock.json", js["Unlock"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusMatchKey.json", js["VersusMatchKey"].ToString(Formatting.None));
                File.AppendAllText("data\\Vip.json", js["Vip"].ToString(Formatting.None));
                File.AppendAllText("data\\Weapon.json", js["Weapon"].ToString(Formatting.None));
                File.AppendAllText("data\\Weather.json", js["Weather"].ToString(Formatting.None));
                File.AppendAllText("data\\versuscpu.json", js["versuscpu"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusMatchCond.json", js["VersusMatchCond"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusWinBonus.json", js["VersusWinBonus"].ToString(Formatting.None));
            }

            using (var sReader = new StreamReader(new FileStream("DataJP/b9cc206f", FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                File.AppendAllText("data\\CampaignChildren.json", js["CampaignChildren"].ToString(Formatting.None));
                File.AppendAllText("data\\CampaignParents.json", js["CampaignParents"].ToString(Formatting.None));
                File.AppendAllText("data\\CampaignTrust.json", js["CampaignTrust"].ToString(Formatting.None));
                File.AppendAllText("data\\GuerrillaShopAdventQuest.json", js["GuerrillaShopAdventQuest"].ToString(Formatting.None));
                File.AppendAllText("data\\GuerrillaShopSchedule.json", js["GuerrillaShopSchedule"].ToString(Formatting.None));
                File.AppendAllText("data\\MapEffect.json", js["MapEffect"].ToString(Formatting.None));
                File.AppendAllText("data\\RaidArea.json", js["RaidArea"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRank.json", js["VersusRank"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRankClass.json", js["VersusRankClass"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRankMission.json", js["VersusRankMission"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRankMissionSchedule.json", js["VersusRankMissionSchedule"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRankRankingReward.json", js["VersusRankRankingReward"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusRankReward.json", js["VersusRankReward"].ToString(Formatting.None));
                File.AppendAllText("data\\VersusDraftUnit.json", js["VersusDraftUnit"].ToString(Formatting.None));
                File.AppendAllText("data\\WeatherSet.json", js["WeatherSet"].ToString(Formatting.None));
                File.AppendAllText("data\\archives.json", js["archives"].ToString(Formatting.None));
                File.AppendAllText("data\\areas.json", js["areas"].ToString(Formatting.None));
                File.AppendAllText("data\\conditions.json", js["conditions"].ToString(Formatting.None));
                File.AppendAllText("data\\magnifications.json", js["magnifications"].ToString(Formatting.None));
                File.AppendAllText("data\\multitowerFloor.json", js["multitowerFloor"].ToString(Formatting.None));
                File.AppendAllText("data\\multitowerRewards.json", js["multitowerRewards"].ToString(Formatting.None));
                File.AppendAllText("data\\multitowers.json", js["multitowers"].ToString(Formatting.None));
                File.AppendAllText("data\\objectives.json", js["objectives"].ToString(Formatting.None));
                File.AppendAllText("data\\parties.json", js["parties"].ToString(Formatting.None));
                File.AppendAllText("data\\questLobbyNews.json", js["questLobbyNews"].ToString(Formatting.None));
                File.AppendAllText("data\\quests.json", js["quests"].ToString(Formatting.None));
                File.AppendAllText("data\\versusrule.json", js["versusrule"].ToString(Formatting.None));
                File.AppendAllText("data\\versusschedule.json", js["versusschedule"].ToString(Formatting.None));
                File.AppendAllText("data\\versusstreakwinbonus.json", js["versusstreakwinbonus"].ToString(Formatting.None));
                File.AppendAllText("data\\versusstreakwinschedule.json", js["versusstreakwinschedule"].ToString(Formatting.None));
                File.AppendAllText("data\\versusTowerFloor.json", js["versusTowerFloor"].ToString(Formatting.None));
                File.AppendAllText("data\\worlds.json", js["worlds"].ToString(Formatting.None));
                File.AppendAllText("data\\versusenabletime.json", js["versusenabletime"].ToString(Formatting.None));
                File.AppendAllText("data\\versusfirstwinbonus.json", js["versusfirstwinbonus"].ToString(Formatting.None));
                File.AppendAllText("data\\versuscamp.json", js["versuscamp"].ToString(Formatting.None));
                File.AppendAllText("data\\versuscoin.json", js["versuscoin"].ToString(Formatting.None));
                File.AppendAllText("data\\versuscoincamp.json", js["versuscoincamp"].ToString(Formatting.None));
                File.AppendAllText("data\\towerRestCost.json", js["towerRestCost"].ToString(Formatting.None));
                File.AppendAllText("data\\towerRewards.json", js["towerRewards"].ToString(Formatting.None));
                File.AppendAllText("data\\towerRoundRewards.json", js["towerRoundRewards"].ToString(Formatting.None));
                File.AppendAllText("data\\towers.json", js["towers"].ToString(Formatting.None));
                File.AppendAllText("data\\towerFloors.json", js["towerFloors"].ToString(Formatting.None));
                File.AppendAllText("data\\towerObjectives.json", js["towerObjectives"].ToString(Formatting.None));
                //File.AppendAllText("data\\simpleDropTable.json", js["simpleDropTable"].ToString(Formatting.None));
                //File.AppendAllText("data\\simpleLocalMaps.json", js["simpleLocalMaps"].ToString(Formatting.None));
                //File.AppendAllText("data\\simpleQuestDrops.json", js["simpleQuestDrops"].ToString(Formatting.None));
                File.AppendAllText("data\\rankingQuestRewards.json", js["rankingQuestRewards"].ToString(Formatting.None));
                File.AppendAllText("data\\rankingQuests.json", js["rankingQuests"].ToString(Formatting.None));
                File.AppendAllText("data\\rankingQuestSchedule.json", js["rankingQuestSchedule"].ToString(Formatting.None));
            }
        }

        public static void Output()
        {
            List<TacTrans> exitlist;
            using (var udb = new LiteDatabase(@"MyData.db"))
            {
                var ucol = udb.GetCollection<TacTrans>();
                exitlist = ucol.FindAll().ToList();
            }

            using (var result = new StreamWriter(new FileStream(@"ResultT.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                foreach (var transe in exitlist)
                {
                    if (transe.JP != transe.CN && transe.CN != null)
                    {
                        result.WriteLine($"{transe.File}\t{transe.Path}\t{transe.IDStr}\t{transe.JP}\t{transe.CN}\t{transe.CreateTime}\t{transe.UpdateTime}");
                    }
                }
            }
        }

        public static void Diff()
        {
            List<CBItem> CB = new List<CBItem>();
            using (var reader = ExcelReaderFactory.CreateReader(File.Open("KeyMinori.xlsx", FileMode.Open, FileAccess.Read)))
            {
                do
                {
                    while (reader.Read())
                    {
                        CB.Add(new CBItem { IDstr = reader.GetString(0), ID = reader.GetString(2), Chinese = reader.GetString(4) });
                    }
                } while (reader.NextResult());
            }

            using (var result = new StreamWriter(new FileStream(@"ResultDiff.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                using (var reader = ExcelReaderFactory.CreateReader(File.Open("Old.xlsx", FileMode.Open, FileAccess.Read)))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var km = CB.First(i => i.IDstr == reader.GetString(0) && i.ID == reader.GetString(2));
                            if (km.Chinese != reader.GetString(4))
                            {
                                result.WriteLine($"{reader.GetString(0)}\t{reader.GetString(2)}\t{km.Chinese}");
                            }
                        }
                    } while (reader.NextResult());
                }
            }
        }

        public static void InitJP()
        {
            Console.WriteLine("初始化日服数据");

            var code = GetWeb("https://jianghanxia.gitee.io/jpalchemisthan/ver");
            var verj = JToken.Parse(GetWeb("https://alchemist.gu3.jp/chkver2", "Post", $"ver={code}"));
            var url = $"https://alchemist-dlc2.gu3.jp/assets/{verj.SelectToken("body.environments.alchemist.assets")}/aatc";

            GetDataAsync($"{url}/ASSETLIST", "ASSETLISTJP_new");

            var colljp = GetCollection("ASSETLISTJP_new");

            GetCollectionFile(url, "ASSETLISTJP", "DataJP", colljp.list);
            GetFileList(colljp.list, "JP.txt");

            GetFileAsync($"{url}/b9cc206f", "DataJP/b9cc206f");
            GetFileAsync($"{url}/49744fd6", "DataJP/49744fd6");

            File.Copy("ASSETLISTJP_new", "ASSETLISTJP", true);
            File.Delete("ASSETLISTJP_new");

            Console.WriteLine("生成日服词表");
            //GetLoc("DataJP", true, colljp.list);
        }

        private static void ZXXCollectionFile()
        {
            var jwp = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("response.json"));

            Directory.CreateDirectory("JPG");
            Parallel.ForEach(jwp._embedded, (item) =>
            {
                GetDataAsync($"https://huiji-public.huijistatic.com/tagatame/uploads/{item.icon_dir}/Portraits_{item.img}.png", $"JPG\\{item.iname}.png");
            });
        }

        public static void InitCN()
        {
            Console.WriteLine("初始化国服数据");

            var code = "50035";
            var url = $"http://update-alccn-prod.ssl.91dena.cn/assets/{code}/aatc";
            GetDataAsync($"{url}/ASSETLIST", "ASSETLISTCN_new");

            var colljp = GetCollection("ASSETLISTCN_new");

            GetCollectionFile(url, "ASSETLISTCN", "DataCN", colljp.list);
            GetFileList(colljp.list, "GF.txt");

            GetFileAsync($"{url}/b2cc6a32", "DataCN/b2cc6a32");
            GetFileAsync($"{url}/64a5ea86", "DataCN/64a5ea86");

            File.Copy("ASSETLISTCN_new", "ASSETLISTCN", true);
            File.Delete("ASSETLISTCN_new");

            Console.WriteLine("生成国服词表");
            GetLoc("DataCN", false, colljp.list);
        }

        private static void JsonList()
        {
            Console.WriteLine("解析JSON数据");
            var qpcn = QuestParam("b2cc6a32", "DataCN/b2cc6a32");
            var qpjp = QuestParam("b9cc206f", "DataJP/b9cc206f");

            var mpcn = MasterParam("64a5ea86", "DataCN/64a5ea86");
            var mpjp = MasterParam("49744fd6", "DataJP/49744fd6");

            Console.WriteLine("抓取WIKI数据");
            List<WikiData> unit;
            if (File.Exists("WikiUnit.txt"))
            {
                unit = JsonConvert.DeserializeObject<WikiJson>(File.ReadAllText("WikiUnit.txt"))._embedded;
            }
            else
            {
                unit = GetWikiData(@"https://tagatame.huijiwiki.com/api/rest_v1/namespace/data?filter={""_id"":{""$regex"":""Data:Unit/""}}&keys={""iname"":1,""name_chs"":1}&count=true");
            }

            unit.ForEach(i => i.Path = $"Unit[?(@.iname=='{i.iname}')].name");

            List<WikiData> jobs;
            if (File.Exists("WikiJobs.txt"))
            {
                jobs = JsonConvert.DeserializeObject<WikiJson>(File.ReadAllText("WikiJobs.txt"))._embedded;
            }
            else
            {
                jobs = GetWikiData(@"https://tagatame.huijiwiki.com/api/rest_v1/namespace/data?filter={""_id"":{""$regex"":""Data:Job/""}}&keys={""iname"":1,""name_chs"":1}&count=true");
            }

            jobs.ForEach(i => i.Path = $"Job[?(@.iname=='{i.iname}')].name");

            var wikidata = new List<WikiData>(unit);
            wikidata.AddRange(jobs);

            Console.WriteLine("合并JSON数据");
            using (var result = new StreamWriter(new FileStream("JSONWord.txt", FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                WriteJsonResult(qpjp, qpcn, wikidata, result);
                WriteJsonResult(mpjp, mpcn, wikidata, result);
            }
        }

        public static List<CBItem> QuestParam(string id, string file)
        {
            List<CBItem> CB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                foreach (var a in js["areas"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"areas[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"areas[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["quests"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].cond", Chinese = $"{a["cond"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"quests[?(@.iname=='{a["iname"]}')].title", Chinese = $"{a["title"]}" });
                }

                foreach (var a in js["towerFloors"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].title", Chinese = $"{a["title"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"towerFloors[?(@.iname=='{a["iname"]}')].cond", Chinese = $"{a["cond"]}" });
                }
            }

            return CB;
        }

        public static List<CBItem> MasterParam(string id, string file)
        {
            List<CBItem> CB = new List<CBItem>();
            using (var sReader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                var js = JToken.Parse(sReader.ReadToEnd());

                foreach (var a in js["Ability"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Ability[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Ability[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Artifact"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Artifact[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Award"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Award[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Award[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Challenge"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Challenge[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["ConceptCard"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"ConceptCard[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"ConceptCard[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Item"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Item[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Job"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Job[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Skill"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Skill[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Skill[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["TobiraCategories"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"TobiraCategories[?(@.category=='{a["category"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Trick"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Trick[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                    CB.Add(new CBItem { IDstr = id, ID = $"Trick[?(@.iname=='{a["iname"]}')].expr", Chinese = $"{a["expr"]}" });
                }

                foreach (var a in js["Trophy"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Trophy[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }

                foreach (var a in js["Unit"].Children())
                {
                    CB.Add(new CBItem { IDstr = id, ID = $"Unit[?(@.iname=='{a["iname"]}')].name", Chinese = $"{a["name"]}" });
                }
            }

            return CB;
        }

        public static List<WikiData> GetWikiData(string url)
        {
            var wu = GetWeb(url);
            var jw = JsonConvert.DeserializeObject<WikiJson>(wu);
            var result = jw._embedded.ToList();

            if (jw._total_pages > 1)
            {
                for (int i = 2; i <= jw._total_pages; i++)
                {
                    var wup = GetWeb($"{url}&page={i}");
                    var jwp = JsonConvert.DeserializeObject<WikiJson>(wup);
                    result.AddRange(jwp._embedded);
                }
            }

            return result;
        }

        public static void WriteJsonResult(List<CBItem> jplist, List<CBItem> cnlist, List<WikiData> wikidata, StreamWriter result)
        {
            foreach (var item in jplist)
            {
                var wi = wikidata.Where(i => i.Path == item.ID);
                if (wi.Any())
                {
                    result.WriteLine($"{item.IDstr}\t{item.ID}\t{wi.First().name_chs}");
                }
                else
                {
                    var h = cnlist.Where(i => i.ID == item.ID);
                    if (h.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(h.First().Chinese))
                        {
                            result.WriteLine($"{item.IDstr}\t{item.ID}\t{h.First().Chinese}");
                        }
                    }
                }
            }
        }

        private static void GetCollectionFile(string url, string oldfile, string dir, List<Item> collection)
        {
            if (File.Exists(oldfile))
            {
                var oldcol = GetCollection(oldfile);

                Directory.CreateDirectory(dir);
                Parallel.ForEach(collection.Where(i => i.Path.StartsWith("Loc/")), (item) =>
                {
                    if (!File.Exists($"{dir}/{item.IDStr}") || !oldcol.list.Any(i => i.ID == item.ID && i.Path == item.Path && i.Hash == item.Hash))
                    {
                        GetFileAsync($"{url}/{item.IDStr}", $"{dir}/{item.IDStr}");
                        Console.WriteLine(item.IDStr);
                    }
                });
            }
            else
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }

                Directory.CreateDirectory(dir);

                Parallel.ForEach(collection.Where(i => i.Path.StartsWith("Loc/")), (item) =>
                {
                    GetFileAsync($"{url}/{item.IDStr}", $"{dir}/{item.IDStr}");
                    Console.WriteLine(item.IDStr);
                });
            }
        }

        public static void GetLoc(string dir, bool jp, List<Item> collection)
        {
            var updateTime = DateTime.Now;

            List<TacTrans> exitlist;
            using (var udb = new LiteDatabase(@"MyData.db"))
            {
                var ucol = udb.GetCollection<TacTrans>();
                exitlist = ucol.FindAll().ToList();
            }

            foreach (var item in collection.Where(i => i.Path.StartsWith("Loc/")).OrderBy(i => i.IDStr))
            {
                Console.WriteLine(item.IDStr);

                using (var file = new StreamReader(new FileStream($"{dir}/{item.IDStr}", FileMode.Open), Encoding.UTF8))
                {
                    while (!file.EndOfStream)
                    {
                        var s = file.ReadLine();

                        var a = s.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (a.Length > 1 && s != "\r")
                        {
                            var id = a[0];
                            using (var db = new LiteDatabase(@"MyData.db"))
                            {
                                var col = db.GetCollection<TacTrans>();
                                if (exitlist.Exists(i => i.Path == item.Path && i.IDStr == id))
                                {
                                    var re = exitlist.First(i => i.Path == item.Path && i.IDStr == id);
                                    if (jp ? re.JP != a[1] : re.CN != a[1])
                                    {
                                        if (jp)
                                        {
                                            re.JP = a[1];
                                        }
                                        else
                                        {
                                            re.CN = a[1];
                                        }

                                        re.UpdateTime = updateTime;

                                        col.Update(re);
                                    }
                                }
                                else
                                {
                                    if (jp)
                                    {
                                        col.Insert(new TacTrans { File = item.IDStr, Path = item.Path, IDStr = a[0], JP = a[1], CreateTime = updateTime, UpdateTime = updateTime });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void GetFileList(List<Item> collection, string filename)
        {
            using (var file = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8))
            {
                foreach (var item in collection)
                {
                    file.WriteLine($"{item.IDStr}\t{item.Flags}\t{item.Path}\t");
                }
            }
        }

        public static (List<Item> list, int ver) GetCollection(string file)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                var revision = binaryReader.ReadInt32();
                var num = binaryReader.ReadInt32();
                var collection = new List<Item>();

                for (int i = 0; i < num; i++)
                {
                    var item = new Item();
                    item.ID = binaryReader.ReadUInt32();
                    item.IDStr = item.ID.ToString("X8").ToLower();
                    item.Size = binaryReader.ReadInt32();
                    item.CompressedSize = binaryReader.ReadInt32();
                    item.Path = binaryReader.ReadString();
                    item.PathHash = binaryReader.ReadInt32();
                    item.Hash = binaryReader.ReadUInt32();
                    item.Flags = (AssetBundleFlags)binaryReader.ReadInt32();

                    int num2 = binaryReader.ReadInt32();
                    for (int j = 0; j < num2; j++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.Dependencies.Add(index);
                    }

                    num2 = binaryReader.ReadInt32();
                    for (int k = 0; k < num2; k++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.AdditionalDependencies.Add(index);
                    }

                    num2 = binaryReader.ReadInt32();
                    for (int l = 0; l < num2; l++)
                    {
                        int index = binaryReader.ReadInt32();
                        item.AdditionalStreamingAssets.Add(index);
                    }

                    collection.Add(item);
                }

                return (collection, revision);
            }
        }

        public static string GetWeb(string url, string method = "Get", string postData = "", int errnum = 0)
        {
            try
            {
                //HTTPSQ请求  
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

                var request = WebRequest.CreateHttp(url);
                request.Method = method.ToUpper();
                request.Accept = "identity";
                request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

                if (method == "Post" && postData != "")
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    var bytes = Encoding.UTF8.GetBytes(postData);
                    var stream = request.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var sReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                    {
                        var res = sReader.ReadToEnd();
                        return res;
                    }
                }
            }
            catch (Exception e)
            {
                if (errnum < 5)
                {
                    Thread.Sleep(2000);
                    errnum += 1;
                    return GetWeb(url, method, postData, errnum);
                }

                throw e;
            }
        }

        public static void GetDataAsync(string url, string filename)
        {
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = request.GetResponse();

            string res;
            using (var steam = response.GetResponseStream())
            {
                using (Stream output = File.OpenWrite(filename))
                {
                    steam.CopyTo(output);
                }
            }

            response.Close();
        }

        public static void GetFileAsync(string url, string filename)
        {
            //HTTPSQ请求  
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            var request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Accept = "identity";
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0; R11/MRA58K)";

            var response = request.GetResponse();

            using (var steam = response.GetResponseStream())
            {
                using (var zlibsteam = new ZlibStream(steam, CompressionMode.Decompress))
                {
                    using (Stream output = File.OpenWrite(filename))
                    {
                        zlibsteam.CopyTo(output);
                    }
                }
            }

            response.Close();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
    }

    public class TacTrans
    {
        public int id { get; set; }

        public string File { get; set; }

        public string IDStr { get; set; }
        public string Path { get; set; }

        public string JP { get; set; }

        public string CN { get; set; }
        public string Trans { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class WikiJson
    {
        public List<WikiData> _embedded { get; set; }
        public string _id { get; set; }
        public int _size { get; set; }
        public int _total_pages { get; set; }
    }

    public class WikiData
    {
        public string _id { get; set; }
        public string iname { get; set; }
        public string name_chs { get; set; }
        public string Path { get; set; }
    }

    public class CBItem
    {
        public string IDstr;
        public string Path;
        public string ID;
        public string Chinese;
    }

    public class Item
    {
        public uint ID;
        public string IDStr;
        public int Size;
        public int CompressedSize;
        public string Path;
        public int PathHash;
        public uint Hash;
        public AssetBundleFlags Flags;
        public List<int> Dependencies = new List<int>();
        public List<int> AdditionalDependencies = new List<int>();
        public List<int> AdditionalStreamingAssets = new List<int>();
        public bool Exist;
    }

    [Flags]
    public enum AssetBundleFlags
    {
        Compressed = 1,
        RawData = 2,
        Required = 4,
        Scene = 8,
        Tutorial = 16,
        Multiplay = 32,
        StreamingAsset = 64,
        TutorialMovie = 128,
        Persistent = 256,
        DiffAsset = 512
    }

    [Serializable]
    public class ChapterParam
    {
        public string iname;
        public string name;
        public string expr;
    }

    [Serializable]
    public class TowerFloorParam
    {
        public string iname;
        public string title;
        public string name;
        public int floor;
        public string tower_id;
        public MapParam[] map;
    }

    [Serializable]
    public class QuestParam
    {
        public string iname;
        public string title;
        public string name;
        public string expr;
        public string area;
        public QuestTypes type;
        public MapParam[] map;
    }

    [Serializable]
    public class MapParam
    {
        public string scn;
        public string set;
        public string bgm;
    }

    public enum QuestTypes
    {
        Story,
        Multi,
        Arena,
        Tutorial,
        Free,
        Event,
        Character,
        Tower,
        VersusFree,
        VersusRank
    }


    public class Rootobject
    {
        public List<_Embedded> _embedded { get; set; }
    }

    public class _Embedded
    {
        public string _id { get; set; }
        public string iname { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string icon_dir { get; set; }
    }
}
