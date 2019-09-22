using System;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Seagull.Core.Helper
{
    /// <summary>
    ///  SectorGoalsKpiProgresTarget service
    /// </summary>
    public partial class CommonHelperService : ICommonHelperService
    {

        #region Fields
        private readonly IStringLocalizer _stringLocalizer;
        #endregion

        #region Constructors

        public CommonHelperService(
            IStringLocalizer stringLocalizer)
        {
            this._stringLocalizer = stringLocalizer;
        }
        #endregion

    }
}
	
