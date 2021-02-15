using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Helpers
{
    public class QuestHelper
    {
        #region Instantiation

        public QuestHelper()
        {
            LoadSkipQuests();
        }

        #endregion

        #region Properties

        public List<int> SkipQuests { get; set; }

        #endregion

        #region Methods

        public void LoadSkipQuests()
        {
            SkipQuests = new List<int>();
            SkipQuests.AddRange(new List<int> { 1520, 1536, 1676, 1677, 1698, 1713, 1714, 1715, 1719, 2123, 2079, 3014, 2084, 3004, 3019, 3281, 5502, 5503, 5512, 6057, 6071, 6360 });
        }

        #endregion

        #region Singleton

        private static QuestHelper _instance;

        public static QuestHelper Instance
        {
            get { return _instance ?? (_instance = new QuestHelper()); }
        }

        #endregion
    }
}
