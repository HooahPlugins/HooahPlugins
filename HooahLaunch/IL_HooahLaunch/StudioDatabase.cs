using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Extractor;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using UnityEngine;

namespace HooahLaunch
{
    public class StudioDatabase
    {
        public static List<string> dataParquet = new List<string>();
        public static string[] dataParquetCached = { };
        public static List<int> dataIndex = new List<int>();
        public static int iterationIndex;

        public static IRatioScorer ratio = ScorerCache.Get<DefaultRatioScorer>();
        public static IRatioScorer partialRatio = ScorerCache.Get<PartialRatioScorer>();
        public static IRatioScorer tokenSet = ScorerCache.Get<TokenSetScorer>();
        public static IRatioScorer partialTokenSet = ScorerCache.Get<PartialTokenSetScorer>();
        public static IRatioScorer tokenSort = ScorerCache.Get<TokenSortScorer>();
        public static IRatioScorer partialTokenSort = ScorerCache.Get<PartialTokenSortScorer>();
        public static IRatioScorer tokenAbbreviation = ScorerCache.Get<TokenAbbreviationScorer>();
        public static IRatioScorer partialTokenAbbreviation = ScorerCache.Get<PartialTokenAbbreviationScorer>();
        public static IRatioScorer weighted = ScorerCache.Get<WeightedRatioScorer>();

        public static void RegisterParquet(string parquet)
        {
            dataParquet.Add(parquet);
            dataIndex.Add(iterationIndex++);
        }

        public static IEnumerator CreateDatabase()
        {
            yield return new WaitUntil(() => KKAPI.Studio.StudioAPI.StudioLoaded);

            var instance = Studio.Info.Instance;
            foreach (var x in instance.dicMapLoadInfo)
            {
                RegisterParquet(x.Value.name);
            }

            foreach (var x in
                from bigGroup in instance.dicItemLoadInfo
                from midGroup in bigGroup.Value
                from x in midGroup.Value
                select x
            )
            {
                RegisterParquet(x.Value.name);
            }

            dataParquetCached = dataParquet.ToArray();
        }

        public static IEnumerable<ExtractedResult<string>> Search(string query,
            IRatioScorer ratio = null,
            int limit = 15)
        {
            return FuzzySharp.Process.ExtractTop(query, dataParquetCached, null, ratio ?? partialRatio, limit);
        }

        public static void CreateCharacterDatabase()
        {
            // Studio.Info.Instance.dicMapLoadInfo.Values.Select(x=>x.name)
        }

        public static void CreateStudioItemDatabase()
        {
        }

        public static void CreateMapDatabase()
        {
        }
    }
}
