using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace newupload2
{
    /// <summary>
    /// Summary description for GetImageHandler
    /// </summary>
    public class GetImageHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int id;
            if (!int.TryParse(context.Request.QueryString["id"], out id))
            {
                context.Response.StatusCode = 400; // Bad Request
                context.Response.End();
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT FileType, FileData FROM ImageTags WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string fileType = reader["FileType"].ToString();
                            byte[] fileData = (byte[])reader["FileData"];

                            context.Response.ContentType = fileType;
                            context.Response.BinaryWrite(fileData);
                        }
                        else
                        {
                            context.Response.StatusCode = 404; // Not Found
                        }
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
