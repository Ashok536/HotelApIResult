using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApiServices
{
    public class CurrencySymbol
    {
        public string symbol = null;

        public string TryGetCurrencySymbol(string ISOCurrencySymbol)
        {

            symbol = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => !c.IsNeutralCulture).Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.LCID);
                }
                catch
                {
                    return null;
                }
            }).Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol).Select(ri => ri.CurrencySymbol).FirstOrDefault();
            return symbol;
        }
    }
}
