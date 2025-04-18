using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite.App_Code
{
   public class CRUD
    {
        //SqlCommand cmd;
        //DataTable dt;
        //SqlDataAdapter adp;
        //DataSet ds;
    
        public static string conStr = WebConfigurationManager.ConnectionStrings["MyScheduleWebsiteConStr"].ConnectionString;
        
        public SqlDataReader getDrPassSql(string mySql) 
        {
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(mySql, con))
            {
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                return dr;
            }
        }
        public SqlDataReader getDrPassSql(string mySql, Dictionary<string, object> myPara)
        {
            SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(mySql, con);

            foreach (var p in myPara)
            {
                cmd.Parameters.AddWithValue(p.Key, p.Value);
            }

            con.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public SqlDataReader getDrViaSpWithPara(string mySPName, Dictionary<string, object> myPara) 
        {
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(mySPName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (KeyValuePair<string, object> p in myPara)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                return dr;
            }
        }
        public DataTable getDT(string mySql) 
        {
            using (SqlConnection con = new SqlConnection(conStr))
            using (con)
            {
                using (SqlCommand cmd = new SqlCommand(mySql, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        con.Open();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public DataTable getDTPassSqlDic(string mySql, Dictionary<string, object> myPara) 
        {
            using (SqlConnection con = new SqlConnection(conStr))
            using (con)
            {
                using (SqlCommand cmd = new SqlCommand(mySql, con))
                {
                    foreach (KeyValuePair<string, object> p in myPara)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        con.Open(); 
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        //public DataTable getDTViaSpWithPara(string storedProcedure, Dictionary<string, object> spInputPara) //6
        //{
        //    using (SqlConnection con = new SqlConnection(conStr))
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        dt = new DataTable();
        //        adp = new SqlDataAdapter();
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                foreach (KeyValuePair<string, object> spData in spInputPara)
        //                {
        //                    cmd.Parameters.AddWithValue(spData.Key, spData.Value);
        //                }
        //                adp.SelectCommand = cmd;
        //                adp.Fill(dt);
        //                return dt;
        //            }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    finally
        //    {
        //        dt.Dispose();
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return dt;
        //}
        public int InsertUpdateDelete(string mySql)
        {

            int rtn = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            using (con)
            {
                using (SqlCommand cmd = new SqlCommand(mySql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    rtn = cmd.ExecuteNonQuery();
                    return rtn;
                }
            }
        }
        public int InsertUpdateDelete(string mySql, Dictionary<string, object> myPara) 
        {
            int rtn = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(mySql, con))
            {
                cmd.CommandType = CommandType.Text;
                foreach (KeyValuePair<string, object> p in myPara)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                using (con)
                {
                    con.Open();
                    rtn = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return rtn;
        }

        public int InsertUpdateDeleteViaSqlDicRtnIdentity(string mySql, Dictionary<string, object> myPara)
        {
            Int32 newIdentityId = 000;
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                using (SqlCommand cmd = new SqlCommand(mySql, con))
                {
                    cmd.CommandType = CommandType.Text;
                    foreach (KeyValuePair<string, object> p in myPara)
                    {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                    using (con)
                    {
                        con.Open();
                        newIdentityId = (Int32.Parse(cmd.ExecuteScalar().ToString()));
                    }
                }
            }

            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            return newIdentityId;
        }
        //public string getPk(string mySql, string ddlSelectedCategory, int ddlSelectedCategoryValue)
        //{
        //    SqlCommand cmd = new SqlCommand(@mySql, con);
        //    cmd.Parameters.AddWithValue(ddlSelectedCategory, ddlSelectedCategoryValue);  
        //    using (con)
        //    {
        //        con.Open();
        //        return cmd.ExecuteScalar().ToString();
        //    }
        //}
        //#region Calling Stored Procedure
        //public DataSet select(string storedProcedure)
        //{
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        ds = new DataSet();
        //        adp = new SqlDataAdapter();
        //        con = new SqlConnection(conStr);
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                adp.Fill(ds);
        //                return ds;
        //            }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        ds.Dispose();
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return ds;
        //}
        //public DataSet select(string storedProcedure, Dictionary<string, object> spInputPara)
        //{
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        ds = new DataSet();
        //        adp = new SqlDataAdapter();
        //        con = new SqlConnection(conStr);
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                foreach (KeyValuePair<string, object> spData in spInputPara)
        //                {
        //                    cmd.Parameters.AddWithValue(spData.Key, spData.Value);
        //                }
        //                adp.SelectCommand = cmd;
        //                adp.Fill(ds);
        //                return ds;
        //            }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        ds.Dispose();
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return ds;
        //}
        public string checkUserExist(string storedProcedure, Dictionary<string, object> spInputPara)
        {
            string strUserName = "";

            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (KeyValuePair<string, object> spData in spInputPara)
                {
                    cmd.Parameters.AddWithValue(spData.Key, spData.Value);
                }

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    strUserName = result.ToString();
                }
                
                
            }

            return strUserName;
        }
        //public string checkUserExist(string storedProcedure, string myUser, string myAppName)
        //{
        //    string strUserName = "";
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(conStr))
        //            cmd = new SqlCommand();
        //        adp = new SqlDataAdapter();
        //        con = new SqlConnection(conStr);
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                cmd.Parameters.AddWithValue("@userName", myUser);
        //                cmd.Parameters.AddWithValue("@appName", myAppName);
        //                strUserName = cmd.ExecuteScalar().ToString();
        //            }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return strUserName;
        //}
        //private SqlDataReader testSp()
        //{
        //    using (SqlConnection con = new SqlConnection(conStr))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("p_SearchDoc", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = "erp";
        //            con.Open();
        //            SqlDataReader dr = cmd.ExecuteReader();
        //            return dr;
        //        }
        //    }
        //}
        //public SqlDataReader getDrViaCmd(string storedProcedure, Dictionary<string, object> spInputPara)
        //{
        //    SqlDataReader dr = null;
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        adp = new SqlDataAdapter();
        //        con = new SqlConnection(conStr);
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                foreach (KeyValuePair<string, object> spData in spInputPara)
        //                {
        //                    cmd.Parameters.AddWithValue(spData.Key, spData.Value);
        //                }
        //                dr = cmd.ExecuteReader();
        //            }
        //        return dr;
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return dr;
        //}
        //#endregion
        //public int checkUserExist2(string storedProcedure, Dictionary<string, object> spInputPara)
        //{
        //    int docId = 0;
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        adp = new SqlDataAdapter();
        //        con = new SqlConnection(conStr);
        //        cmd.CommandText = storedProcedure;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = con;
        //        if (cmd.Connection.State == ConnectionState.Closed)
        //            using (con)
        //            {
        //                cmd.Connection.Open();
        //                foreach (KeyValuePair<string, object> spData in spInputPara)
        //                {
        //                    cmd.Parameters.AddWithValue(spData.Key, spData.Value);
        //                }
        //                docId = int.Parse(cmd.ExecuteScalar().ToString());
        //           }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    finally
        //    {
        //        cmd.Connection.Close();
        //        cmd.Dispose();
        //    }
        //    return docId;
        //}
        public bool authenticateUser(string mySql, Dictionary<string, object> formValues)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(mySql, con))
            {
                foreach (KeyValuePair<string, object> p in formValues)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read();
                }
            }
        }
        //public void populatComboViaDr(string mySql, DropDownList comboName, string myValue, string myText)
        //{
        //    CRUD myCrud = new CRUD();
        //    using (SqlDataReader dr = myCrud.getDrPassSql(mySql))
        //    {
        //        comboName.DataTextField = myText.ToString();
        //        comboName.DataValueField = myValue.ToString();
        //        comboName.DataSource = dr;
        //        comboName.DataBind();
        //    }
        //}

        
        //public void populateCombo(DropDownList myDDL, string mySql, string myDataValueField, string myDataTextField)
        //{
        //    using (SqlDataReader dr = this.getDrPassSql(mySql))
        //    {
        //        myDDL.DataValueField = myDataValueField;
        //        myDDL.DataTextField = myDataTextField;
        //        myDDL.DataSource = dr;
        //        myDDL.DataBind();
        //    }
        //}
        //public void populateCombo(DropDownList myDDL, string mySql, string myDataValueField, string myDataTextField, Dictionary<string, object> myPara)
        //{
        //    using (SqlDataReader dr = this.getDrPassSql(mySql, myPara))
        //    {
        //        myDDL.DataValueField = myDataValueField;
        //        myDDL.DataTextField = myDataTextField;
        //        myDDL.DataSource = dr;
        //        myDDL.DataBind();
        //    }
        //}
        public void populateGv(GridView myGv, string mySql)
        {
            using (SqlDataReader dr = this.getDrPassSql(mySql))
            {
                myGv.DataSource = dr;
                myGv.DataBind();
            }
        }
        public void populateGvDr(GridView myGv, string mySql)
        {
            using (SqlDataReader dr = this.getDrPassSql(mySql))
            {
                myGv.DataSource = dr;
                myGv.DataBind();
            }
        }
        public void populateGvDT(GridView myGv, string mySql)
        {
            DataTable dt = this.getDT(mySql);
            myGv.DataSource = dt;
            myGv.DataBind();
        }
        public void populateGv(GridView myGv, string mySql, string myDataTextField, Dictionary<string, object> myPara)
        {
            using (SqlDataReader dr = this.getDrPassSql(mySql, myPara))
            {
                myGv.DataSource = dr;
                myGv.DataBind();
            }
        }
        

        public static void clearAllPools()
        {
            SqlConnection.ClearAllPools();
        }


    }

}
