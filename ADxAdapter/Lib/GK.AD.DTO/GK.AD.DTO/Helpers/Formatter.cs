using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.AD.DTO
{
    // ================================================================================
    public class Formatter
    {
        // -----------------------------------------------------------------------------
        public static string ToString(IADObject adobj, bool compact = true)
        {
            if (adobj == null)
            {
                return string.Format("DTO:ADO=[NULL]");
            }
            else
            {
                return string.Format(
                    "DTO:ADO=[{0}/{1}/{2}]",
                    adobj.objectClass.ToString().ToUpperInvariant(),
                    adobj.objectGuid.ToString().ToUpperInvariant(),
                    adobj.name
                    );
            }
        }
    }
}
