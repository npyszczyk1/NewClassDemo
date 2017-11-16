<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" 
    AutoEventWireup="true" CodeFile="AlbumsByYearRelease.aspx.cs" 
    Inherits="SamplePages_AlbumsByYearRelease" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <h1> Albums By Year Released</h1>
    <asp:Label ID="Label1" runat="server" Text="Enter minimum year"></asp:Label>
    <asp:TextBox ID="minyear" runat="server"></asp:TextBox>
    <asp:Label ID="Label2" runat="server" Text="Enter maximum year"></asp:Label>
    <asp:TextBox ID="maxyear" runat="server"></asp:TextBox>
    <asp:LinkButton ID="LinkButton1" runat="server">Submit</asp:LinkButton>
    <br />
    <asp:GridView ID="AlbumsList" runat="server" 
        AutoGenerateColumns="False" DataSourceID="AlbumsListODS" 
        AllowPaging="True">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title"></asp:BoundField>
            <asp:BoundField DataField="ReleaseYear" HeaderText="ReleaseYear" SortExpression="ReleaseYear"></asp:BoundField>
            <asp:BoundField DataField="ReleaseLabel" HeaderText="ReleaseLabel" SortExpression="ReleaseLabel"></asp:BoundField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="AlbumsListODS" runat="server" 
        OldValuesParameterFormatString="original_{0}" 
        SelectMethod="Albums_ListByYearRelease" 
        TypeName="ChinookSystem.BLL.AlbumController">
        <SelectParameters>
            <asp:ControlParameter ControlID="minyear" 
                PropertyName="Text" DefaultValue="1900" 
                Name="minyear" Type="Int32"></asp:ControlParameter>
            <asp:ControlParameter ControlID="maxyear"
                 PropertyName="Text" DefaultValue="2017" 
                Name="maxyear" Type="Int32"></asp:ControlParameter>
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

