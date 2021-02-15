using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.DAL.EF.Helpers;

namespace OpenNos.DAL.DAO
{
    public class ActPartDAO : IActPartDAO
    {
        #region Methods

        public void Insert(List<ActPartDTO> actParts)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (ActPartDTO actPart in actParts)
                    {
                        ActPart entity = new ActPart();
                        context.ActPart.Add(entity);
                    }
                    context.Configuration.AutoDetectChangesEnabled = true;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}