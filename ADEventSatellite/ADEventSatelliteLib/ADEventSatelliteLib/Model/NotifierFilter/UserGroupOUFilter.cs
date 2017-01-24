using GK.AD;
using System.Linq;

namespace ADEventSatellite.Model
{
    // ================================================================================
    public class UserGroupOUFilter : INotifierDropFilter
    {
        static ObjectClass[] _objectOKClasses = new ObjectClass[] { ObjectClass.user, ObjectClass.group, ObjectClass.organizationalUnit };

        // -----------------------------------------------------------------------------
        public IADObject FilterObject(IADObject adObj)
        {
            if (adObj == null)
                return null;

            return _objectOKClasses.Contains<ObjectClass>(adObj.objectClass) ? adObj : null;
        }
    }
}