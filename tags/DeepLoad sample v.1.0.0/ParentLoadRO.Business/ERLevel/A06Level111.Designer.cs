using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ParentLoadRO.Business.ERLevel
{

    /// <summary>
    /// A06Level111 (read only object).<br/>
    /// This is a generated base class of <see cref="A06Level111"/> business object.
    /// </summary>
    /// <remarks>
    /// This class contains one child collection:<br/>
    /// - <see cref="A07Level1111Objects"/> of type <see cref="A07Level1111Coll"/> (1:M relation to <see cref="A08Level1111"/>)<br/>
    /// This class is an item of <see cref="A05Level111Coll"/> collection.
    /// </remarks>
    [Serializable]
    public partial class A06Level111 : ReadOnlyBase<A06Level111>
    {

        #region State Fields

        [NotUndoable]
        private byte[] _rowVersion = new byte[] {};

        [NotUndoable]
        [NonSerialized]
        internal int marentID1 = 0;

        #endregion

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="Level_1_1_1_ID"/> property.
        /// </summary>
        public static readonly PropertyInfo<int> Level_1_1_1_IDProperty = RegisterProperty<int>(p => p.Level_1_1_1_ID, "Level_1_1_1 ID", -1);
        /// <summary>
        /// Gets the Level_1_1_1 ID.
        /// </summary>
        /// <value>The Level_1_1_1 ID.</value>
        public int Level_1_1_1_ID
        {
            get { return GetProperty(Level_1_1_1_IDProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="Level_1_1_1_Name"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> Level_1_1_1_NameProperty = RegisterProperty<string>(p => p.Level_1_1_1_Name, "Level_1_1_1 Name");
        /// <summary>
        /// Gets the Level_1_1_1 Name.
        /// </summary>
        /// <value>The Level_1_1_1 Name.</value>
        public string Level_1_1_1_Name
        {
            get { return GetProperty(Level_1_1_1_NameProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="MarentID1"/> property.
        /// </summary>
        public static readonly PropertyInfo<int> MarentID1Property = RegisterProperty<int>(p => p.MarentID1, "Marent ID1");
        /// <summary>
        /// Gets the Marent ID1.
        /// </summary>
        /// <value>The Marent ID1.</value>
        public int MarentID1
        {
            get { return GetProperty(MarentID1Property); }
        }

        /// <summary>
        /// Maintains metadata about child <see cref="A07Level1111SingleObject"/> property.
        /// </summary>
        public static readonly PropertyInfo<A07Level1111Child> A07Level1111SingleObjectProperty = RegisterProperty<A07Level1111Child>(p => p.A07Level1111SingleObject, "A7 Level1111 Single Object");
        /// <summary>
        /// Gets the A07 Level1111 Single Object ("parent load" child property).
        /// </summary>
        /// <value>The A07 Level1111 Single Object.</value>
        public A07Level1111Child A07Level1111SingleObject
        {
            get { return GetProperty(A07Level1111SingleObjectProperty); }
            private set { LoadProperty(A07Level1111SingleObjectProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about child <see cref="A07Level1111ASingleObject"/> property.
        /// </summary>
        public static readonly PropertyInfo<A07Level1111ReChild> A07Level1111ASingleObjectProperty = RegisterProperty<A07Level1111ReChild>(p => p.A07Level1111ASingleObject, "A7 Level1111 ASimple Object");
        /// <summary>
        /// Gets the A07 Level1111 ASingle Object ("parent load" child property).
        /// </summary>
        /// <value>The A07 Level1111 ASingle Object.</value>
        public A07Level1111ReChild A07Level1111ASingleObject
        {
            get { return GetProperty(A07Level1111ASingleObjectProperty); }
            private set { LoadProperty(A07Level1111ASingleObjectProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about child <see cref="A07Level1111Objects"/> property.
        /// </summary>
        public static readonly PropertyInfo<A07Level1111Coll> A07Level1111ObjectsProperty = RegisterProperty<A07Level1111Coll>(p => p.A07Level1111Objects, "A7 Level1111 Objects");
        /// <summary>
        /// Gets the A07 Level1111 Objects ("parent load" child property).
        /// </summary>
        /// <value>The A07 Level1111 Objects.</value>
        public A07Level1111Coll A07Level1111Objects
        {
            get { return GetProperty(A07Level1111ObjectsProperty); }
            private set { LoadProperty(A07Level1111ObjectsProperty, value); }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Loads a <see cref="A06Level111"/> object from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        /// <returns>A reference to the fetched <see cref="A06Level111"/> object.</returns>
        internal static A06Level111 GetA06Level111(SafeDataReader dr)
        {
            A06Level111 obj = new A06Level111();
            obj.Fetch(dr);
            obj.LoadProperty(A07Level1111ObjectsProperty, new A07Level1111Coll());
            return obj;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="A06Level111"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        private A06Level111()
        {
            // Prevent direct creation
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads a <see cref="A06Level111"/> object from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        private void Fetch(SafeDataReader dr)
        {
            // Value properties
            LoadProperty(Level_1_1_1_IDProperty, dr.GetInt32("Level_1_1_1_ID"));
            LoadProperty(Level_1_1_1_NameProperty, dr.GetString("Level_1_1_1_Name"));
            LoadProperty(MarentID1Property, dr.GetInt32("MarentID1"));
            _rowVersion = (dr.GetValue("RowVersion")) as byte[];
            marentID1 = dr.GetInt32("MarentID1");
            var args = new DataPortalHookArgs(dr);
            OnFetchRead(args);
        }

        /// <summary>
        /// Loads child <see cref="A07Level1111Child"/> object.
        /// </summary>
        /// <param name="child">The child object to load.</param>
        internal void LoadChild(A07Level1111Child child)
        {
            LoadProperty(A07Level1111SingleObjectProperty, child);
        }

        /// <summary>
        /// Loads child <see cref="A07Level1111ReChild"/> object.
        /// </summary>
        /// <param name="child">The child object to load.</param>
        internal void LoadChild(A07Level1111ReChild child)
        {
            LoadProperty(A07Level1111ASingleObjectProperty, child);
        }

        #endregion

        #region Pseudo Events

        /// <summary>
        /// Occurs after the low level fetch operation, before the data reader is destroyed.
        /// </summary>
        partial void OnFetchRead(DataPortalHookArgs args);

        #endregion

    }
}
