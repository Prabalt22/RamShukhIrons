using System.Data;
using MvcApp.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DAL
{
	private readonly string _connectionStr;

	public DAL(string connectionStr)
	{
		_connectionStr = connectionStr;
	}

	public List<Iron> GetIronList()
	{

        List<Iron> irons = new List<Iron>();
        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetIronList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {

                    while (dr != null && dr.Read())
                    {
                        irons.Add(new Iron
                        {
                            Id = (int)dr["Id"],
                            IronName = dr["IronName"].ToString(),
                            IronDescription = dr["IronDescription"].ToString(),
                            Price = (decimal)dr["Price"],
                            Brand = dr["Brand"].ToString(),
                            ImageData = dr["ImageData"] != DBNull.Value ? dr["ImageData"].ToString() : null
                        });
                    }
                }
            }
        }    
            
        return irons;
    }


    public bool RegisterUser(Registration model)
    {

        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                conn.Open();

                int rowAffected = cmd.ExecuteNonQuery();

                return rowAffected > 0;
               
            }
        }
       
    }


    public Dictionary<string, string> LoginUser(Login model)
    {
        Dictionary<string, string> UserCred = new Dictionary<string, string>();

        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_LoginUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        UserCred.Add("Name", dr["Name"].ToString());
                        UserCred.Add("Email", dr["Email"].ToString());
                    }

                }

            }
        }
        return UserCred;
    }


    public Iron GetIronDetails(int Id)
    {

        Iron ironDetails = null;
        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetIronDetails", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        ironDetails = new Iron()
                        {
                            Id = (int)dr["Id"],
                            IronName = dr["IronName"].ToString(),
                            IronDescription = dr["IronDescription"].ToString(),
                            Price = (decimal)dr["Price"],
                            Brand = dr["Brand"].ToString(),
                            ImageData = dr["ImageData"] != DBNull.Value ? dr["ImageData"].ToString() : null,
                        };
                    }

                }
            }
        }

        return ironDetails;
    }

    public bool AddNewBlog(Blog model, string Email)
    {

        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_AddNewBlog", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Content", model.Content);
                cmd.Parameters.AddWithValue("@Email", Email);
                conn.Open();

                int rowAffected = cmd.ExecuteNonQuery();

                return rowAffected > 0;

            }
        }
    }


    public List<Blog> GetAllComments()
    {

        List<Blog> blogs = new List<Blog>();
        using (SqlConnection conn = new SqlConnection(_connectionStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetAllComments", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    
                    while (dr != null && dr.Read())
                    {
                        blogs.Add(new Blog
                        {
                            Id = (int)dr["Id"],
                            Title = dr["Title"].ToString(),
                            Content = dr["Content"].ToString(),
                            Email = dr["Email"].ToString(),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                        });
                    }

                }
            }
        }

        return blogs;
    }
}
