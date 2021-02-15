using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.DAL.Interface
{
    public interface IActPartDAO
    {
        #region Methods

        void Insert(List<ActPartDTO> actParts);

        #endregion
    }
}