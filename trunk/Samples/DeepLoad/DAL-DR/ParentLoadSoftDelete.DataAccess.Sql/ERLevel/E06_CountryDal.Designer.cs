using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using ParentLoadSoftDelete.DataAccess;
using ParentLoadSoftDelete.DataAccess.ERLevel;

namespace ParentLoadSoftDelete.DataAccess.Sql.ERLevel
{
    /// <summary>
    /// DAL SQL Server implementation of <see cref="IE06_CountryDal"/>
    /// </summary>
    public partial class E06_CountryDal : IE06_CountryDal
    {
        /// <summary>
        /// Inserts a new E06_Country object in the database.
        /// </summary>
        /// <param name="parent_SubContinent_ID">The parent Parent Sub Continent ID.</param>
        /// <param name="country_ID">The Country ID.</param>
        /// <param name="country_Name">The Country Name.</param>
        /// <returns>The Row Version of the new E06_Country.</returns>
        public byte[] Insert(int parent_SubContinent_ID, out int country_ID, string country_Name)
        {
            country_ID = -1;
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("AddE06_Country", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Parent_SubContinent_ID", parent_SubContinent_ID).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@Country_ID", country_ID).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Country_Name", country_Name).DbType = DbType.String;
                    cmd.Parameters.Add("@NewRowVersion", SqlDbType.Timestamp).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    country_ID = (int)cmd.Parameters["@Country_ID"].Value;
                    return (byte[])cmd.Parameters["@NewRowVersion"].Value;
                }
            }
        }

        /// <summary>
        /// Updates in the database all changes made to the E06_Country object.
        /// </summary>
        /// <param name="country_ID">The Country ID.</param>
        /// <param name="country_Name">The Country Name.</param>
        /// <param name="parentSubContinentID">The Parent Sub Continent ID.</param>
        /// <param name="rowVersion">The Row Version.</param>
        /// <returns>The updated Row Version.</returns>
        public byte[] Update(int country_ID, string country_Name, int parentSubContinentID, byte[] rowVersion)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("UpdateE06_Country", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Country_ID", country_ID).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@Country_Name", country_Name).DbType = DbType.String;
                    cmd.Parameters.AddWithValue("@Parent_SubContinent_ID", parentSubContinentID).DbType = DbType.Int32;
                    cmd.Parameters.AddWithValue("@RowVersion", rowVersion).DbType = DbType.Binary;
                    cmd.Parameters.Add("@NewRowVersion", SqlDbType.Timestamp).Direction = ParameterDirection.Output;
                    var rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        throw new DataNotFoundException("E06_Country");

                    return (byte[])cmd.Parameters["@NewRowVersion"].Value;
                }
            }
        }

        /// <summary>
        /// Deletes the E06_Country object from database.
        /// </summary>
        /// <param name="country_ID">The Country ID.</param>
        public void Delete(int country_ID)
        {
            using (var ctx = ConnectionManager<SqlConnection>.GetManager("DeepLoad"))
            {
                using (var cmd = new SqlCommand("DeleteE06_Country", ctx.Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Country_ID", country_ID).DbType = DbType.Int32;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
