using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IActDAO
    {
        #region Methods

        void Insert(List<ActDTO> acts);

        #endregion
    }
}