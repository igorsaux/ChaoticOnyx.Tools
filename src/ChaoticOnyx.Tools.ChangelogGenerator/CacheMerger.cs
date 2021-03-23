#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public static class CacheMerger
    {
        /// <summary>
        ///     Объединение чейнджлогов совпадающих по датам и авторам.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<Changelog> Merge(List<Changelog> changes)
        {
            List<Changelog> result = new();

            foreach (var change in changes)
            {
                if (result.Any(e => e.Author == change.Author && e.Date.Date == change.Date.Date))
                {
                    continue;
                }
                
                var changelogChanges = changes.FindAll(e => e.Author == change.Author 
                                                            && e.Date.Date == change.Date.Date)
                                              .SelectMany(e => e.Changes).ToHashSet();

                result.Add(new()
                {
                    Author = change.Author,
                    Date = change.Date,
                    Changes = changelogChanges.ToList()
                });
            }

            return result;
        }
    }
}
