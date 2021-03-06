using System;
using System.Data;
using Csla;

namespace SelfLoad.DataAccess.ERCLevel
{
    /// <summary>
    /// DAL Interface for D03_Continent_ReChild type
    /// </summary>
    public partial interface ID03_Continent_ReChildDal
    {
        /// <summary>
        /// Loads a D03_Continent_ReChild object from the database.
        /// </summary>
        /// <param name="continent_ID2">The Continent ID2.</param>
        /// <returns>A data reader to the D03_Continent_ReChild.</returns>
        IDataReader Fetch(int continent_ID2);

        /// <summary>
        /// Inserts a new D03_Continent_ReChild object in the database.
        /// </summary>
        /// <param name="continent_ID2">The parent Continent ID2.</param>
        /// <param name="continent_Child_Name">The Continent Child Name.</param>
        void Insert(int continent_ID2, string continent_Child_Name);

        /// <summary>
        /// Updates in the database all changes made to the D03_Continent_ReChild object.
        /// </summary>
        /// <param name="continent_ID2">The parent Continent ID2.</param>
        /// <param name="continent_Child_Name">The Continent Child Name.</param>
        void Update(int continent_ID2, string continent_Child_Name);

        /// <summary>
        /// Deletes the D03_Continent_ReChild object from database.
        /// </summary>
        /// <param name="continent_ID2">The parent Continent ID2.</param>
        void Delete(int continent_ID2);
    }
}
