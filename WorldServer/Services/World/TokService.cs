using Common;
using FrameWork;
using System.Collections.Generic;

namespace WorldServer.Services.World
{
    [Service]
    public class TokService : ServiceBase
    {
        public static Dictionary<ushort, tok_infos> _Toks;
        public static List<tok_infos> DiscoveringToks;
        public static Dictionary<ushort, tok_bestiary> _ToksBestary;

        [LoadingFunction(true)]
        public static void LoadTok_Infos()
        {
            Log.Debug("WorldMgr", "Loading LoadTok_Infos...");

            _Toks = new Dictionary<ushort, tok_infos>();

            IList<tok_infos> IToks = Database.SelectAllObjects<tok_infos>();
            DiscoveringToks = new List<tok_infos>();

            if (IToks != null)
            {
                foreach (tok_infos Info in IToks)
                {
                    _Toks.Add(Info.Entry, Info);
                    if (Info.EventName.Contains("discovered") || Info.EventName.Contains("unlocked"))
                    {
                        DiscoveringToks.Add(Info);
                    }
                }
            }

            Log.Success("LoadTok_Infos", "Loaded " + _Toks.Count + " Tok_Infos");
        }

        [LoadingFunction(true)]
        public static void LoadTok_Bestary()
        {
            Log.Debug("WorldMgr", "Loading LoadTok_Bestary...");

            _ToksBestary = new Dictionary<ushort, tok_bestiary>();

            IList<tok_bestiary> IToks = Database.SelectAllObjects<tok_bestiary>();

            if (IToks != null)
            {
                foreach (tok_bestiary Info in IToks)
                {
                    _ToksBestary.Add(Info.Creature_Sub_Type, Info);
                }
            }

            Log.Success("LoadTok_Bestary", "Loaded " + _ToksBestary.Count + " Tok_Bestary");
        }

        public static tok_infos GetTok(ushort Entry)
        {
            tok_infos tok;
            _Toks.TryGetValue(Entry, out tok);
            return tok;
        }

        public static tok_bestiary GetTokBestary(ushort subTypeId)
        {
            tok_bestiary bestiary;
            _ToksBestary.TryGetValue(subTypeId, out bestiary);
            return bestiary;
        }
    }
}