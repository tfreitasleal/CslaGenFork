//  This file was generated by CSLA Object Generator - CslaGenFork v4.5
//
// Filename:    User
// ObjectType:  User
// CSLAType:    EditableRoot

using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using DocStore.Business.Util;
using Csla.Rules;
using Csla.Rules.CommonRules;
using DocStore.Business.Security;

namespace DocStore.Business.Admin
{

    /// <summary>
    /// User (editable root object).<br/>
    /// This is a generated base class of <see cref="User"/> business object.
    /// </summary>
    [Serializable]
    public partial class User : BusinessBase<User>
    {

        #region Static Fields

        private static int _lastId;

        #endregion

        #region Business Properties

        /// <summary>
        /// Maintains metadata about <see cref="UserID"/> property.
        /// </summary>
        [NotUndoable]
        public static readonly PropertyInfo<int> UserIDProperty = RegisterProperty<int>(p => p.UserID, "User ID");
        /// <summary>
        /// Gets the User ID.
        /// </summary>
        /// <value>The User ID.</value>
        public int UserID
        {
            get { return GetProperty(UserIDProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="Name"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name, "Name");
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>The Name.</value>
        public string Name
        {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="Login"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> LoginProperty = RegisterProperty<string>(p => p.Login, "Login");
        /// <summary>
        /// Gets or sets the Login.
        /// </summary>
        /// <value>The Login.</value>
        public string Login
        {
            get { return GetProperty(LoginProperty); }
            set { SetProperty(LoginProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="Picture"/> property.
        /// </summary>
        public static readonly PropertyInfo<byte[]> PictureProperty = RegisterProperty<byte[]>(p => p.Picture, "Picture");
        /// <summary>
        /// Gets or sets the Picture.
        /// </summary>
        /// <value>The Picture.</value>
        public byte[] Picture
        {
            get { return GetProperty(PictureProperty); }
            set { SetProperty(PictureProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="Email"/> property.
        /// </summary>
        public static readonly PropertyInfo<string> EmailProperty = RegisterProperty<string>(p => p.Email, "Email");
        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        /// <value>The Email.</value>
        public string Email
        {
            get { return GetProperty(EmailProperty); }
            set { SetProperty(EmailProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="IsActive"/> property.
        /// </summary>
        public static readonly PropertyInfo<bool> IsActiveProperty = RegisterProperty<bool>(p => p.IsActive, "IsActive");
        /// <summary>
        /// Gets or sets the active or deleted state.
        /// </summary>
        /// <value><c>true</c> if IsActive; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return GetProperty(IsActiveProperty); }
            set { SetProperty(IsActiveProperty, value); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="CreateDate"/> property.
        /// </summary>
        public static readonly PropertyInfo<SmartDate> CreateDateProperty = RegisterProperty<SmartDate>(p => p.CreateDate, "Create Date");
        /// <summary>
        /// Gets the Create Date.
        /// </summary>
        /// <value>The Create Date.</value>
        public SmartDate CreateDate
        {
            get { return GetProperty(CreateDateProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="CreateUserID"/> property.
        /// </summary>
        public static readonly PropertyInfo<int> CreateUserIDProperty = RegisterProperty<int>(p => p.CreateUserID, "Create User ID");
        /// <summary>
        /// Gets the Create User ID.
        /// </summary>
        /// <value>The Create User ID.</value>
        public int CreateUserID
        {
            get { return GetProperty(CreateUserIDProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="ChangeDate"/> property.
        /// </summary>
        public static readonly PropertyInfo<SmartDate> ChangeDateProperty = RegisterProperty<SmartDate>(p => p.ChangeDate, "Change Date");
        /// <summary>
        /// Gets the Change Date.
        /// </summary>
        /// <value>The Change Date.</value>
        public SmartDate ChangeDate
        {
            get { return GetProperty(ChangeDateProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="ChangeUserID"/> property.
        /// </summary>
        public static readonly PropertyInfo<int> ChangeUserIDProperty = RegisterProperty<int>(p => p.ChangeUserID, "Change User ID");
        /// <summary>
        /// Gets the Change User ID.
        /// </summary>
        /// <value>The Change User ID.</value>
        public int ChangeUserID
        {
            get { return GetProperty(ChangeUserIDProperty); }
        }

        /// <summary>
        /// Maintains metadata about <see cref="RowVersion"/> property.
        /// </summary>
        [NotUndoable]
        public static readonly PropertyInfo<byte[]> RowVersionProperty = RegisterProperty<byte[]>(p => p.RowVersion, "Row Version");
        /// <summary>
        /// Gets the Row Version.
        /// </summary>
        /// <value>The Row Version.</value>
        public byte[] RowVersion
        {
            get { return GetProperty(RowVersionProperty); }
        }

        /// <summary>
        /// Gets the Create User Name.
        /// </summary>
        /// <value>The Create User Name.</value>
        public string CreateUserName
        {
            get
            {
                var result = string.Empty;
                if (UserNVL.GetUserNVL().ContainsKey(CreateUserID))
                    result = UserNVL.GetUserNVL().GetItemByKey(CreateUserID).Value;
                return result;
            }
        }

        /// <summary>
        /// Gets the Change User Name.
        /// </summary>
        /// <value>The Change User Name.</value>
        public string ChangeUserName
        {
            get
            {
                var result = string.Empty;
                if (UserNVL.GetUserNVL().ContainsKey(ChangeUserID))
                    result = UserNVL.GetUserNVL().GetItemByKey(ChangeUserID).Value;
                return result;
            }
        }

        #endregion

        #region BusinessBase<T> overrides

        /// <summary>
        /// Returns a string that represents the current <see cref="User"/>
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            // Return the Primary Key as a string
            return Name.ToString();
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method. Creates a new <see cref="User"/> object.
        /// </summary>
        /// <returns>A reference to the created <see cref="User"/> object.</returns>
        public static User NewUser()
        {
            return DataPortal.Create<User>();
        }

        /// <summary>
        /// Factory method. Loads a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID parameter of the User to fetch.</param>
        /// <returns>A reference to the fetched <see cref="User"/> object.</returns>
        public static User GetUser(int userID)
        {
            return DataPortal.Fetch<User>(userID);
        }

        /// <summary>
        /// Factory method. Deletes a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID of the User to delete.</param>
        public static void DeleteUser(int userID)
        {
            DataPortal.Delete<User>(userID);
        }

        /// <summary>
        /// Factory method. Undeletes a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID of the User to undelete.</param>
        /// <returns>A reference to the undeleted <see cref="User"/> object.</returns>
        public static User UndeleteUser(int userID)
        {
            var obj = DataPortal.Fetch<User>(userID);
            obj.IsActive = true;
            return obj.Save();
        }

        /// <summary>
        /// Factory method. Asynchronously creates a new <see cref="User"/> object.
        /// </summary>
        /// <param name="callback">The completion callback method.</param>
        public static void NewUser(EventHandler<DataPortalResult<User>> callback)
        {
            DataPortal.BeginCreate<User>(callback);
        }

        /// <summary>
        /// Factory method. Asynchronously loads a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID parameter of the User to fetch.</param>
        /// <param name="callback">The completion callback method.</param>
        public static void GetUser(int userID, EventHandler<DataPortalResult<User>> callback)
        {
            DataPortal.BeginFetch<User>(userID, callback);
        }

        /// <summary>
        /// Factory method. Asynchronously deletes a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID of the User to delete.</param>
        /// <param name="callback">The completion callback method.</param>
        public static void DeleteUser(int userID, EventHandler<DataPortalResult<User>> callback)
        {
            DataPortal.BeginDelete<User>(userID, callback);
        }

        /// <summary>
        /// Factory method. Asynchronously undeletes a <see cref="User"/> object, based on given parameters.
        /// </summary>
        /// <param name="userID">The UserID of the User to undelete.</param>
        /// <param name="callback">The completion callback method.</param>
        public static void UndeleteUser(int userID, EventHandler<DataPortalResult<User>> callback)
        {
            var obj = new User();
            DataPortal.BeginFetch<User>(userID, (o, e) =>
                {
                    if (e.Error != null)
                        throw e.Error;
                    else
                        obj = e.Object;
                });
            obj.IsActive = true;
            obj.BeginSave(callback);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public User()
        {
            // Use factory methods and do not use direct creation.
        }

        #endregion

        #region Object Authorization

        /// <summary>
        /// Adds the object authorization rules.
        /// </summary>
        protected static void AddObjectAuthorizationRules()
        {
            BusinessRules.AddRule(typeof (User), new IsInRole(AuthorizationActions.CreateObject, "Manager"));
            BusinessRules.AddRule(typeof (User), new IsInRole(AuthorizationActions.GetObject, "User"));
            BusinessRules.AddRule(typeof (User), new IsInRole(AuthorizationActions.EditObject, "Manager"));
            BusinessRules.AddRule(typeof (User), new IsInRole(AuthorizationActions.DeleteObject, "Admin"));

            AddObjectAuthorizationRulesExtend();
        }

        /// <summary>
        /// Allows the set up of custom object authorization rules.
        /// </summary>
        static partial void AddObjectAuthorizationRulesExtend();

        /// <summary>
        /// Checks if the current user can create a new User object.
        /// </summary>
        /// <returns><c>true</c> if the user can create a new object; otherwise, <c>false</c>.</returns>
        public static bool CanAddObject()
        {
            return BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(User));
        }

        /// <summary>
        /// Checks if the current user can retrieve User's properties.
        /// </summary>
        /// <returns><c>true</c> if the user can read the object; otherwise, <c>false</c>.</returns>
        public static bool CanGetObject()
        {
            return BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.GetObject, typeof(User));
        }

        /// <summary>
        /// Checks if the current user can change User's properties.
        /// </summary>
        /// <returns><c>true</c> if the user can update the object; otherwise, <c>false</c>.</returns>
        public static bool CanEditObject()
        {
            return BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(User));
        }

        /// <summary>
        /// Checks if the current user can delete a User object.
        /// </summary>
        /// <returns><c>true</c> if the user can delete the object; otherwise, <c>false</c>.</returns>
        public static bool CanDeleteObject()
        {
            return BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.DeleteObject, typeof(User));
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Loads default values for the <see cref="User"/> object properties.
        /// </summary>
        [RunLocal]
        protected override void DataPortal_Create()
        {
            LoadProperty(UserIDProperty, System.Threading.Interlocked.Decrement(ref _lastId));
            LoadProperty(PictureProperty, new byte[0]);
            LoadProperty(CreateDateProperty, new SmartDate(DateTime.Now));
            LoadProperty(CreateUserIDProperty, UserInformation.UserId);
            LoadProperty(ChangeDateProperty, ReadProperty(CreateDateProperty));
            LoadProperty(ChangeUserIDProperty, ReadProperty(CreateUserIDProperty));
            var args = new DataPortalHookArgs();
            OnCreate(args);
            base.DataPortal_Create();
        }

        /// <summary>
        /// Loads a <see cref="User"/> object from the database, based on given criteria.
        /// </summary>
        /// <param name="userID">The User ID.</param>
        protected void DataPortal_Fetch(int userID)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager(Database.DocStoreConnection, false))
            {
                using (var cmd = new SqlCommand("GetUser", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userID).DbType = DbType.Int32;
                    var args = new DataPortalHookArgs(cmd, userID);
                    OnFetchPre(args);
                    Fetch(cmd);
                    OnFetchPost(args);
                }
            }
            // check all object rules and property rules
            BusinessRules.CheckRules();
        }

        private void Fetch(SqlCommand cmd)
        {
            using (var dr = new SafeDataReader(cmd.ExecuteReader()))
            {
                if (dr.Read())
                {
                    Fetch(dr);
                }
            }
        }

        /// <summary>
        /// Loads a <see cref="User"/> object from the given SafeDataReader.
        /// </summary>
        /// <param name="dr">The SafeDataReader to use.</param>
        private void Fetch(SafeDataReader dr)
        {
            // Value properties
            LoadProperty(UserIDProperty, dr.GetInt32("UserID"));
            LoadProperty(NameProperty, dr.GetString("Name"));
            LoadProperty(LoginProperty, dr.GetString("Login"));
            LoadProperty(EmailProperty, dr.GetString("Email"));
            LoadProperty(IsActiveProperty, dr.GetBoolean("IsActive"));
            LoadProperty(CreateDateProperty, dr.GetSmartDate("CreateDate", true));
            LoadProperty(CreateUserIDProperty, dr.GetInt32("CreateUserID"));
            LoadProperty(ChangeDateProperty, dr.GetSmartDate("ChangeDate", true));
            LoadProperty(ChangeUserIDProperty, dr.GetInt32("ChangeUserID"));
            LoadProperty(RowVersionProperty, dr.GetValue("RowVersion") as byte[]);
            var args = new DataPortalHookArgs(dr);
            OnFetchRead(args);
        }

        /// <summary>
        /// Inserts a new <see cref="User"/> object in the database.
        /// </summary>
        protected override void DataPortal_Insert()
        {
            SimpleAuditTrail();
            using (var ctx = TransactionManager<SqlConnection, SqlTransaction>.GetManager(Database.DocStoreConnection, false))
            {
                using (var cmd = new SqlCommand("AddUser", ctx.Connection))
                {
                    cmd.Transaction = ctx.Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", ReadProperty(UserIDProperty)).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Name", ReadProperty(NameProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@Login", ReadProperty(LoginProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@Email", ReadProperty(EmailProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@IsActive", ReadProperty(IsActiveProperty)).DbType = DbType.Boolean;
                    cmd.Parameters.AddWithValue("@CreateDate", ReadProperty(CreateDateProperty).DBValue).DbType = DbType.DateTime2;
                    cmd.Parameters.AddWithValue("@CreateUserID", ReadProperty(CreateUserIDProperty)).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@ChangeDate", ReadProperty(ChangeDateProperty).DBValue).DbType = DbType.DateTime2;
                    cmd.Parameters.AddWithValue("@ChangeUserID", ReadProperty(ChangeUserIDProperty)).DbType = DbType.Int32;
                    cmd.Parameters.Add("@NewRowVersion", SqlDbType.Timestamp).Direction = ParameterDirection.Output;
                    var args = new DataPortalHookArgs(cmd);
                    OnInsertPre(args);
                    cmd.ExecuteNonQuery();
                    OnInsertPost(args);
                    LoadProperty(UserIDProperty, (int) cmd.Parameters["@UserID"].Value);
                    LoadProperty(RowVersionProperty, (byte[]) cmd.Parameters["@NewRowVersion"].Value);
                }
                ctx.Commit();
            }
        }

        /// <summary>
        /// Updates in the database all changes made to the <see cref="User"/> object.
        /// </summary>
        protected override void DataPortal_Update()
        {
            SimpleAuditTrail();
            using (var ctx = TransactionManager<SqlConnection, SqlTransaction>.GetManager(Database.DocStoreConnection, false))
            {
                using (var cmd = new SqlCommand("UpdateUser", ctx.Connection))
                {
                    cmd.Transaction = ctx.Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", ReadProperty(UserIDProperty)).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@Name", ReadProperty(NameProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@Login", ReadProperty(LoginProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@Email", ReadProperty(EmailProperty)).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@IsActive", ReadProperty(IsActiveProperty)).DbType = DbType.Boolean;
                    cmd.Parameters.AddWithValue("@ChangeDate", ReadProperty(ChangeDateProperty).DBValue).DbType = DbType.DateTime2;
                    cmd.Parameters.AddWithValue("@ChangeUserID", ReadProperty(ChangeUserIDProperty)).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@RowVersion", ReadProperty(RowVersionProperty)).DbType = DbType.Binary;
                    cmd.Parameters.Add("@NewRowVersion", SqlDbType.Timestamp).Direction = ParameterDirection.Output;
                    var args = new DataPortalHookArgs(cmd);
                    OnUpdatePre(args);
                    cmd.ExecuteNonQuery();
                    OnUpdatePost(args);
                    LoadProperty(RowVersionProperty, (byte[]) cmd.Parameters["@NewRowVersion"].Value);
                }
                ctx.Commit();
            }
        }

        private void SimpleAuditTrail()
        {
            LoadProperty(ChangeDateProperty, DateTime.Now);
            LoadProperty(ChangeUserIDProperty, UserInformation.UserId);
            OnPropertyChanged("ChangeUserName");
            if (IsNew)
            {
                LoadProperty(CreateDateProperty, ReadProperty(ChangeDateProperty));
                LoadProperty(CreateUserIDProperty, ReadProperty(ChangeUserIDProperty));
                OnPropertyChanged("CreateUserName");
            }
        }

        /// <summary>
        /// Self deletes the <see cref="User"/> object.
        /// </summary>
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(UserID);
        }

        /// <summary>
        /// Deletes the <see cref="User"/> object from database.
        /// </summary>
        /// <param name="userID">The delete criteria.</param>
        protected void DataPortal_Delete(int userID)
        {
            // audit the object, just in case soft delete is used on this object
            SimpleAuditTrail();
            using (var ctx = TransactionManager<SqlConnection, SqlTransaction>.GetManager(Database.DocStoreConnection, false))
            {
                using (var cmd = new SqlCommand("DeleteUser", ctx.Connection))
                {
                    cmd.Transaction = ctx.Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userID).DbType = DbType.Int32;
                    var args = new DataPortalHookArgs(cmd, userID);
                    OnDeletePre(args);
                    cmd.ExecuteNonQuery();
                    OnDeletePost(args);
                }
                ctx.Commit();
            }
        }

        #endregion

        #region DataPortal Hooks

        /// <summary>
        /// Occurs after setting all defaults for object creation.
        /// </summary>
        partial void OnCreate(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Delete, after setting query parameters and before the delete operation.
        /// </summary>
        partial void OnDeletePre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Delete, after the delete operation, before Commit().
        /// </summary>
        partial void OnDeletePost(DataPortalHookArgs args);

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

        /// <summary>
        /// Occurs after setting query parameters and before the update operation.
        /// </summary>
        partial void OnUpdatePre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after the update operation, before setting back row identifiers (RowVersion) and Commit().
        /// </summary>
        partial void OnUpdatePost(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after setting query parameters and before the insert operation.
        /// </summary>
        partial void OnInsertPre(DataPortalHookArgs args);

        /// <summary>
        /// Occurs in DataPortal_Insert, after the insert operation, before setting back row identifiers (ID and RowVersion) and Commit().
        /// </summary>
        partial void OnInsertPost(DataPortalHookArgs args);

        #endregion

    }
}
