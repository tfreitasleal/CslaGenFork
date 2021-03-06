using System;
using System.Data;
using Csla;

namespace SelfLoadSoftDelete.DataAccess.ERLevel
{
    /// <summary>
    /// DAL Interface for G07_Country_Child type
    /// </summary>
    public partial interface IG07_Country_ChildDal
    {
        /// <summary>
        /// Loads a G07_Country_Child object from the database.
        /// </summary>
        /// <param name="country_ID1">The Country ID1.</param>
        /// <returns>A data reader to the G07_Country_Child.</returns>
        IDataReader Fetch(int country_ID1);

        /// <summary>
        /// Inserts a new G07_Country_Child object in the database.
        /// </summary>
        /// <param name="country_ID1">The parent Country ID1.</param>
        /// <param name="country_Child_Name">The Country Child Name.</param>
        void Insert(int country_ID1, string country_Child_Name);

        /// <summary>
        /// Updates in the database all changes made to the G07_Country_Child object.
        /// </summary>
        /// <param name="country_ID1">The parent Country ID1.</param>
        /// <param name="country_Child_Name">The Country Child Name.</param>
        void Update(int country_ID1, string country_Child_Name);

        /// <summary>
        /// Deletes the G07_Country_Child object from database.
        /// </summary>
        /// <param name="country_ID1">The parent Country ID1.</param>
        void Delete(int country_ID1);
    }
}
