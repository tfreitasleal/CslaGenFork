using System;
using Csla;
using SelfLoadRO.DataAccess;
using SelfLoadRO.DataAccess.ERLevel;

namespace SelfLoadRO.Business.ERLevel
{

    /// <summary>
    /// C05_SubContinent_Child (read only object).<br/>
    /// This is a generated base class of <see cref="C05_SubContinent_Child"/> business object.
    /// </summary>
    /// <remarks>
    /// This class is an item of <see cref="C04_SubContinent"/> collection.
    /// </remarks>
    [Serializable]
    public partial class C05_SubContinent_Child : ReadOnlyBase<C05_SubContinent_Child>
    {

        #region State Fields

        private byte[] _rowVersion = new byte[] {};

        #endregion

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="SubContinent_Child_Name"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> SubContinent_Child_NameProperty = RegisterProperty<string>(p => p.SubContinent_Child_Name, "Sub Continent Child Name");
        /// <summary>
        /// Gets the Sub Continent Child Name.
        /// </summary>
        /// <value>The Sub Continent Child Name.</value>
        public string SubContinent_Child_Name
        {
            get { return GetProperty(SubContinent_Child_NameProperty); }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Loads a <see cref="C05_SubContinent_Child"/> object, based on given parameters.
        /// </summary>
        /// <param name="parentSubContinent_ID1">The ParentSubContinent_ID1 parameter of the C05_SubContinent_Child to fetch.</param>
        /// <returns>A reference to the fetched <see cref="C05_SubContinent_Child"/> object.</returns>
        internal static C05_SubContinent_Child GetC05_SubContinent_Child(int parentSubContinent_ID1)
        {
            return DataPortal.FetchChild<C05_SubContinent_Child>(parentSubContinent_ID1);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="C05_SubContinent_Child"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public C05_SubContinent_Child()
        {
            // Use factory methods and do not use direct creation.
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="C05_SubContinent_Child"/> object from the database, based on given criteria.
        /// </summary>
        /// <param name="parentSubContinent_ID1">The Parent Sub Continent ID1.</param>
        protected void Child_Fetch(int parentSubContinent_ID1)
        {
            var args = new DataPortalHookArgs(parentSubContinent_ID1);
            OnFetchPre(args);
            using (var dalManager = DalFactorySelfLoadRO.GetManager())
            {
                var dal = dalManager.GetProvider<IC05_SubContinent_ChildDal>();
                var data = dal.Fetch(parentSubContinent_ID1);
                Fetch(data);
            }
            OnFetchPost(args);
        }

        /// <summary>
        /// Loads a <see cref="C05_SubContinent_Child"/> object from the given <see cref="C05_SubContinent_ChildDto"/>.
        /// </summary>
        /// <param name="data">The C05_SubContinent_ChildDto to use.</param>
        private void Fetch(C05_SubContinent_ChildDto data)
        {
            // Value properties
            LoadProperty(SubContinent_Child_NameProperty, data.SubContinent_Child_Name);
            _rowVersion = data.RowVersion;
            var args = new DataPortalHookArgs(data);
            OnFetchRead(args);
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

        /// <summary>
        /// Occurs after the low level fetch operation, before the data reader is destroyed.
        /// </summary>
        partial void OnFetchRead(DataPortalHookArgs args);

        #endregion

    }
}
