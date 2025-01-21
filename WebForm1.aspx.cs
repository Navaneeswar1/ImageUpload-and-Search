using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace newupload2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }



        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string searchInput = SearchInput.Text.Trim();
            if (string.IsNullOrEmpty(searchInput))
            {
                SearchStatus.Text = "Please enter a search term.";
                return;
            }

            // Remove the file extension from the search term (if any)
            string searchBaseName = Path.GetFileNameWithoutExtension(searchInput).Trim();

            if (string.IsNullOrEmpty(searchBaseName))
            {
                SearchStatus.Text = "Please enter a valid search term.";
                return;
            }

            // Convert the search term to lowercase for case-insensitive search
            string lowerSearchBaseName = searchBaseName.ToLower();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            List<dynamic> results = new List<dynamic>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Adjust the query to search for filenames that exactly match (ignoring extension)
                string query = @"
            SELECT Id, FileName, Keywords
            FROM ImageTags
            WHERE LOWER(FileName) = @SearchTerm
            OR LOWER(Keywords) LIKE @SearchKeyword";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Match file names that exactly match the lowercase search term (before extensions)
                    command.Parameters.AddWithValue("@SearchTerm", lowerSearchBaseName);

                    // Search in keywords for the lowercase term
                    string searchKeyword = "%" + lowerSearchBaseName + "%";
                    command.Parameters.AddWithValue("@SearchKeyword", searchKeyword);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new
                            {
                                Id = reader["Id"].ToString(),
                                FileName = reader["FileName"].ToString(),
                                Keywords = reader["Keywords"].ToString()
                            });
                        }
                    }
                }
            }

            if (results.Count > 0)
            {
                SearchResultsRepeater.DataSource = results;
                SearchResultsRepeater.DataBind();
                SearchStatus.Text = $"{results.Count} result(s) found.";
            }
            else
            {
                SearchResultsRepeater.DataSource = null;
                SearchResultsRepeater.DataBind();
                SearchStatus.Text = "No results found.";
            }
        }







        protected void Button1_Click(object sender, EventArgs e)
        {
            if ((FileUpload1.PostedFile != null) && (FileUpload1.PostedFile.ContentLength > 0))
            {
                string userInputName = DesiredFileName.Text.Trim();
                string keywords = Keywords.Text.Trim();

                if (string.IsNullOrEmpty(userInputName))
                {
                    FileUploadStatus.Text = "Please enter a valid file name.";
                    return;
                }

                if (string.IsNullOrEmpty(keywords))
                {
                    FileUploadStatus.Text = "Please enter at least one keyword.";
                    return;
                }

                string[] keywordArray = keywords.Split(',');
                if (keywordArray.Length > 5)
                {
                    FileUploadStatus.Text = "You can only enter up to 5 keywords.";
                    return;
                }

                string fileExtension = Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt" };

                if (Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    // Remove extension from the user input name
                    string renamedFileName = Path.GetFileNameWithoutExtension(userInputName); // Remove any existing extension from user input

                    // Check if the filename already exists in the database
                    if (IsFileNameExists(renamedFileName))
                    {
                        FileUploadStatus.Text = $"A file with the name '{userInputName}' already exists. Please choose a different name.";
                        return;
                    }

                    try
                    {
                        byte[] fileData = FileUpload1.FileBytes;
                        string fileType = FileUpload1.PostedFile.ContentType;

                        // Save file details and keywords in the database
                        SaveToDatabase(renamedFileName, fileType, fileData, keywords);

                        // Successfully uploaded, reset all fields
                        ResetFields();

                        FileUploadStatus.Text = $"The file '{renamedFileName}' has been uploaded and tagged successfully.";
                    }
                    catch (Exception ex)
                    {
                        FileUploadStatus.Text = "Error: " + ex.Message;
                    }
                }
                else
                {
                    FileUploadStatus.Text = "Only files with the following extensions are allowed: .jpg, .jpeg, .png, .gif, .pdf, .txt.";
                }
            }
            else
            {
                FileUploadStatus.Text = "Please select a file to upload.";
            }
        }

        // Helper method to reset all fields
        private void ResetFields()
        {
            DesiredFileName.Text = string.Empty;  // Clear the file name input
            Keywords.Text = string.Empty;         // Clear the keywords input
            FileUpload1.Attributes.Clear();       // Clear the file upload control
            FileUploadStatus.Text = string.Empty; // Clear the upload status label
        }



        // Helper method to check if the filename exists
        private bool IsFileNameExists(string fileName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM ImageTags WHERE FileName = @FileName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FileName", fileName);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0; // Return true if the filename exists
                }
            }
        }


        private void SaveToDatabase(string fileName, string fileType, byte[] fileData, string keywords)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO ImageTags (FileName, FileType, FileData, Keywords) VALUES (@FileName, @FileType, @FileData, @Keywords)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@FileType", fileType);
                    command.Parameters.AddWithValue("@FileData", fileData);
                    command.Parameters.AddWithValue("@Keywords", keywords);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}

