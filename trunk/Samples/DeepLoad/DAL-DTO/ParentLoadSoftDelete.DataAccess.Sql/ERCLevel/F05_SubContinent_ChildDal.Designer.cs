using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using ParentLoadSoftDelete.DataAccess;
using ParentLoadSoftDelete.DataAccess.ERCLevel;

namespace ParentLoadSoftDelete.DataAccess.Sql.ERCLevel
{
    /// <summary>
    /// DAL SQL Server implementation of <see cref="IF05_SubContinent_ChildDal"/>
    /// </summary>
    public partial class F05_SubContinent_ChildDal : IF05_SubContinent_ChildDal
    {
        /// <summary>
        /// Inserts a new F05_SubContinent_Child object in the database.
        /// </summary>
        /// <param name="f05_SubContinent_Child">The F05 Sub Continent Child DTO.</param>
        /// <returns>The new <see cref="F05_SubContinent_ChildDto"/>.</returns>
        public F05_SubContinent_ChildDto Insert(F05_SubContinent_ChildDto f05_SubContinent_Child)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("AddF05_SubContinent_Child", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubContinent_ID1", f05_SubContinent_Child.Parent_SubContinent_ID).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@SubContinent_Child_Name", f05_SubContinent_Child.SubContinent_Child_Name).DbType = DbType.String;
                    cmd.ExecuteNonQuery();
                }
            }
            return f05_SubContinent_Child;
        }

        /// <summary>
        /// Updates in the database all changes made to the F05_SubContinent_Child object.
        /// </summary>
        /// <param name="f05_SubContinent_Child">The F05 Sub Continent Child DTO.</param>
        /// <returns>The updated <see cref="F05_SubContinent_ChildDto"/>.</returns>
        public F05_SubContinent_ChildDto Update(F05_SubContinent_ChildDto f05_SubContinent_Child)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("UpdateF05_SubContinent_Child", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubContinent_ID1", f05_SubContinent_Child.Parent_SubContinent_ID).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@SubContinent_Child_Name", f05_SubContinent_Child.SubContinent_Child_Name).DbType = DbType.String;
                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new DataNotFoundException("F05_SubContinent_Child");
                }
            }
            return f05_SubContinent_Child;
        }

        /// <summary>
        /// Deletes the F05_SubContinent_Child object from database.
        /// </summary>
        /// <param name="subContinent_ID1">The parent Sub Continent ID1.</param>
        public void Delete(int subContinent_ID1)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("DeleteF05_SubContinent_Child", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubContinent_ID1", subContinent_ID1).DbType = DbType.Int32;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
