using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace SelfLoad.Business.ERLevel
{

    /// <summary>
    /// C07_RegionColl (editable child list).<br/>
    /// This is a generated base class of <see cref="C07_RegionColl"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is child of <see cref="C06_Country"/> editable child object.<br/>
    /// The items of the collection are <see cref="C08_Region"/> objects.
    /// </remarks>
    [Serializable]
    public partial class C07_RegionColl : BusinessListBase<C07_RegionColl, C08_Region>
    {

        #region Collection Business Methods

        /// <summary>
        /// Removes a <see cref="C08_Region"/> item from the collection.
        /// </summary>
        /// <param name="region_ID">The Region_ID of the item to be removed.</param>
        public void Remove(int region_ID)
        {
            foreach (var c08_Region in this)
            {
                if (c08_Region.Region_ID == region_ID)
                {
                    Remove(c08_Region);
                    break;
                }
            }
        }

        /// <summary>
        /// Determines whether a <see cref="C08_Region"/> item is in the collection.
        /// </summary>
        /// <param name="region_ID">The Region_ID of the item to search for.</param>
        /// <returns><c>true</c> if the C08_Region is a collection item; otherwise, <c>false</c>.</returns>
        public bool Contains(int region_ID)
        {
            foreach (var c08_Region in this)
            {
                if (c08_Region.Region_ID == region_ID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether a <see cref="C08_Region"/> item is in the collection's DeletedList.
        /// </summary>
        /// <param name="region_ID">The Region_ID of the item to search for.</param>
        /// <returns><c>true</c> if the C08_Region is a deleted collection item; otherwise, <c>false</c>.</returns>
        public bool ContainsDeleted(int region_ID)
        {
            foreach (var c08_Region in DeletedList)
            {
                if (c08_Region.Region_ID == region_ID)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Find Methods

        /// <summary>
        /// Finds a <see cref="C08_Region"/> item of the <see cref="C07_RegionColl"/> collection, based on a given Region_ID.
        /// </summary>
        /// <param name="region_ID">The Region_ID.</param>
        /// <returns>A <see cref="C08_Region"/> object.</returns>
        public C08_Region FindC08_RegionByRegion_ID(int region_ID)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].Region_ID.Equals(region_ID))
                {
                    return this[i];
                }
            }

            return null;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Creates a new <see cref="C07_RegionColl"/> collection.
        /// </summary>
        /// <returns>A reference to the created <see cref="C07_RegionColl"/> collection.</returns>
        internal static C07_RegionColl NewC07_RegionColl()
        {
            return DataPortal.CreateChild<C07_RegionColl>();
        }

        /// <summary>
        /// Factory method. Loads a <see cref="C07_RegionColl"/> collection, based on given parameters.
        /// </summary>
        /// <param name="parent_Country_ID">The Parent_Country_ID parameter of the C07_RegionColl to fetch.</param>
        /// <returns>A reference to the fetched <see cref="C07_RegionColl"/> collection.</returns>
        internal static C07_RegionColl GetC07_RegionColl(int parent_Country_ID)
        {
            return DataPortal.FetchChild<C07_RegionColl>(parent_Country_ID);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="C07_RegionColl"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public C07_RegionColl()
        {
            // Use factory methods and do not use direct creation.

            // show the framework that this is a child object
            MarkAsChild();

            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            AllowNew = true;
            AllowEdit = true;
            AllowRemove = true;
            RaiseListChangedEvents = rlce;
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="C07_RegionColl"/> collection from the database, based on given criteria.
        /// </summary>
        /// <param name="parent_Country_ID">The Parent Country ID.</param>
        protected void Child_Fetch(int parent_Country_ID)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("GetC07_RegionColl", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Parent_Country_ID", parent_Country_ID).DbType = DbType.Int32;
                    var args = new DataPortalHookArgs(cmd, parent_Country_ID);
                    OnFetchPre(args);
                    LoadCollection(cmd);
                    OnFetchPost(args);
                }
            }
            foreach (var item in this)
            {
                item.FetchChildren();
            }
        }

        private void LoadCollection(SqlCommand cmd)
        {
            using (var dr = new SafeDataReader(cmd.ExecuteReader()))
            {
                Fetch(dr);
            }
        }

        /// <summary>
        /// Loads all <see cref="C07_RegionColl"/> collection items from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        private void Fetch(SafeDataReader dr)
        {
            var rlce = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            while (dr.Read())
            {
                Add(C08_Region.GetC08_Region(dr));
            }
            RaiseListChangedEvents = rlce;
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after setting query parameters and before the fetch operation.
        /// </summary>
        partial void OnFetchPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs after the fetch operation (object or collection is fully loaded and set up).
        /// </summary>
        partial void OnFetchPost(DataPortalHookArgs args);

        #endregion

    }
}
