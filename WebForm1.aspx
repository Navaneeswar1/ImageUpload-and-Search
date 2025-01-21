<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="newupload2.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Image Upload and Search</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }

        form {
            width: 80%;
            margin: 20px auto;
            background-color: #ffffff;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 5px;
            box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.2);
        }

        div {
            margin-bottom: 15px;
        }

        p {
            margin: 5px 0;
            font-size: 14px;
        }

        input[type="text"], .aspNetDisabled {
            width: 100%;
            padding: 8px;
            margin-top: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        input[type="file"] {
            margin-top: 5px;
        }

        button, input[type="submit"], .aspNetDisabled {
            padding: 10px 20px;
            background-color: #007bff;
            color: #ffffff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        button:hover, input[type="submit"]:hover {
            background-color: #0056b3;
        }

        .search-results {
            margin-top: 20px;
        }

        .search-results div {
            border: 1px solid #ddd;
            padding: 15px;
            margin-bottom: 10px;
            background-color: #fafafa;
            border-radius: 4px;
        }

        .search-results img {
            max-width: 100%;
            height: auto;
            display: block;
            margin-bottom: 10px;
        }

        .status-label {
            color: #ff0000;
            font-weight: bold;
        }

        .success-label {
            color: #28a745;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <!-- Search Section -->
        <div>
            <p>Search by Name or Keyword:</p>
            <asp:TextBox ID="SearchInput" runat="server" />
            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
        </div>

        <!-- Search Results -->
        <div class="search-results">
            <asp:Label ID="SearchStatus" runat="server" CssClass="status-label"></asp:Label>
            <asp:Repeater ID="SearchResultsRepeater" runat="server">
                <ItemTemplate>
                    <div>
                        <!-- Render image from a dynamic handler -->
                        <img src="GetImageHandler.ashx?id=<%# Eval("Id") %>" alt='<%# Eval("FileName") %>' />
                        <p><strong>Name:</strong> <%# Eval("FileName") %></p>
                        <p><strong>Keywords:</strong> <%# Eval("Keywords") %></p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Upload Section -->
        <div>
            <p>Enter Desired File Name (without extension):</p>
            <asp:TextBox ID="DesiredFileName" runat="server" />
        </div>

        <div>
            <p>Enter up to 5 Keywords (comma-separated):</p>
            <asp:TextBox ID="Keywords" runat="server" />
        </div>

        <div>  
            <p>Welcome to the webform1 where you can Browse to Upload File </p>
            <asp:FileUpload ID="FileUpload1" runat="server" />  
        </div>

        <p>  
            <asp:Button ID="Button1" runat="server" Text="Upload File" OnClick="Button1_Click" />  
        </p>

        <p>  
            <asp:Label runat="server" ID="FileUploadStatus" CssClass="success-label"></asp:Label>  
        </p>  
    </form>
</body>
</html>
