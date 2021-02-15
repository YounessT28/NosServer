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
    public class ActDAO : IActDAO
    {
        #region Methods

        public void Insert(List<ActDTO> acts)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    foreach (ActDTO act in acts)
                    {
                        Act entity = new Act();
                        context.Act.Add(entity);
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