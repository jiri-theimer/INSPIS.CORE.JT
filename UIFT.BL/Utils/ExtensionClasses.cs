using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Konverze DateTime typu do formatu BL vrstvy (string)
        /// </summary>
        /// <param name="datum">Datum a cas ke konverzi</param>
        /// <returns>Konvertovany datum a cas ve formatu pro BL</returns>
        public static string ConvertToBL(this DateTime date)
        {
            return date.ToString("s");
        }

        /// <summary>
        /// Converts time to Unix Timestamp
        /// </summary>
        /// <returns>Milliseconds since Epoch</returns>
        public static long ConvertToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Convert.ToInt64(diff.TotalMilliseconds);
        }
    }

    public static class StringHelper
    {
        public static string ToSingleLine(this List<BO.StringPair> list)
        {
            return string.Join(", ", list.Select(t => t.Value).ToArray());
        }

        /// <summary>
        /// Konverze datumu z BL vrstvy do datetime typu
        /// </summary>
        /// <param name="datum">Datum a cas ulozene ve formatu pro BL</param>
        /// <returns>Konvertovany DateTime typ</returns>
        public static DateTime ConvertFromBL(this string datum)
        {
            return System.DateTime.Parse(datum);
        }
    }

    public static class BLFactoryHelper
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Vyhodnoceni eval bool expression
        /// </summary>
        public static bool BoolEvaluator(this BL.Factory factory, int a11id, string stringToEvaluate)
        {
            if (!string.IsNullOrEmpty(stringToEvaluate))
            {
                var evaluator = new EVAL.Evaluator(factory, a11id);
                object ret = evaluator.TryEval(stringToEvaluate);
                
                if (ret != null) // evaluator vratil nejaky vysledek
                {
                    if (ret is bool)
                    {
                        return (bool)ret;
                    }
                    else
                    {
                        log.Error("BLFactoryHelper.BoolEvaluator: return value is not boolean; a11id={0}; {1}", a11id, stringToEvaluate);
                    }
                }
                return false;
            }
            else
                return false;
        }
    }
}