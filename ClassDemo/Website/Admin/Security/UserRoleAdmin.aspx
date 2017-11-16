<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
     CodeFile="UserRoleAdmin.aspx.cs" Inherits="Admin_Security_UserRoleAdmin" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
     <h1>CRUD in Tabs</h1>
       <!--Common Update panel for user message control -->
    <asp:UpdatePanel ID="UpdatePanelMessage" runat="server">
        <ContentTemplate>
            <uc1:MessageUserControl runat="server" ID="MessageUserControl" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="row">
        <div class="col-md-12">
          <!--script for tab to tab movement -->
            <script>
                function nextButton(anchorRef) {
                    $('a[href="' + anchorRef + '"]').tab('show');
                }
            </script>
            <!--Nav tabs-->
            <ul class="nav nav-tabs">
                <li class="active"><a href="#user" data-toggle="tab">User</a></li>
                <li ><a href="#role" data-toggle="tab">Role</a></li>
            </ul>

            <!--Tab panes one for each tab-->
            <div class="tab-content"> 
                <div class="tab-pane fade in active" id="user">
                    <asp:UpdatePanel ID="UpdatePanelUser" runat="server">
                        <ContentTemplate>
                            <asp:ListView ID="UserListView" runat="server" 
                                DataSourceID="UserListViewODS"
                                InsertItemPosition="LastItem"
                                ItemType="Chinook.Data.Entities.Security.UserProfile"
                                DataKeyNames="UserId"
                                OnItemInserting="UserListView_ItemInserting"
                                OnItemDeleted="RefreshAll"
                                OnItemInserted="RefreshAll">
                            <EmptyDataTemplate>
                                <span>No Security users have been set up.</span>
                            </EmptyDataTemplate>
                            <LayoutTemplate>
                                <div class="row bginfo">
                                    <div class="col-sm-2 h4">Action</div>
                                    <div class="col-sm-2 h4">User Names</div>
                                    <div class="col-sm-3 h4">Profile</div>
                                    <div class="col-sm-2 "></div>
                                    <div class="col-sm-3 h4">Roles</div>
                                </div>
                                <div runat="server" id="itemPlaceHolder">
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:LinkButton ID="RemoveUser" runat="server" 
                                            CommandName="Delete">Remove</asp:LinkButton>
                                    </div>
                                    <div class="col-sm-2">
                                        <%# Item.UserName %>
                                    </div>
                                    <div class="col-sm-5">
                                        <%# Item.Email %>&nbsp;&nbsp;
                                        <%# Item.FirstName + " " + Item.LastName %>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:Repeater ID="RoleUserReapter" runat="server"
                                            DataSource="<%# Item.RoleMemberships%>"
                                            ItemType="System.String">
                                            <ItemTemplate>
                                                    <%# Item %>
                                            </ItemTemplate>
                                            <SeparatorTemplate>, </SeparatorTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <InsertItemTemplate>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:LinkButton ID="InsertUser" runat="server" 
                                        CommandName="Insert">Insert</asp:LinkButton>
                                        <asp:LinkButton ID="CancelButton" runat="server" 
                                        CommandName="Cancel">Cancel</asp:LinkButton>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="UserName" runat="server"
                                            text='<%# BindItem.UserName %>' 
                                            placeholder="User Name"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="UserEmail" runat="server"
                                            text='<%# BindItem.Email %>' TextMode="Email" 
                                            placeholder="User Email"></asp:TextBox>
                                    </div>
                                    
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="EmployeeID" runat="server"
                                            text='<%# BindItem.EmployeeId %>' TextMode="Number" 
                                            placeholder="Employee ID" ></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:CheckBoxList ID="RoleMemberships" runat="server"
                                            DataSourceID="RoleNameODS"></asp:CheckBoxList>
                                    </div>
                                </div>
                            </InsertItemTemplate>
                        </asp:ListView>
                        <asp:ObjectDataSource ID="UserListViewODS" runat="server" 
                            DataObjectTypeName="Chinook.Data.Entities.Security.UserProfile" 
                            DeleteMethod="RemoveUser" 
                            InsertMethod="AddUser" 
                            SelectMethod="ListAllUsers"
                            OldValuesParameterFormatString="original_{0}"  
                            TypeName="ChinookSystem.BLL.Security.UserManager"
                             OnDeleted="CheckForException"
                             OnInserted="CheckForException"
                             OnSelected="CheckForException">
                        </asp:ObjectDataSource>
                        <asp:ObjectDataSource ID="RoleNameODS" runat="server" 
                            SelectMethod="ListAllRoleNames"
                            OldValuesParameterFormatString="original_{0}"  
                            TypeName="ChinookSystem.BLL.Security.RoleManager">
                        </asp:ObjectDataSource>
                        </ContentTemplate>
                    </asp:UpdatePanel>   
                </div>
                <div class="tab-pane fade" id="role">
                    <asp:UpdatePanel ID="UpdatePanelrole" runat="server">
                        <ContentTemplate>
                           <asp:ListView ID="RoleListView" runat="server"
                               DataSourceID="RoleODS"
                               InsertItemPosition="LastItem"
                               DataKeyNames="RoleID"
                               ItemType="Chinook.Data.Entities.Security.RoleProfile"
                               OnItemDeleted="RefreshAll"
                               OnItemInserted="RefreshAll"
                               >
                               <EditItemTemplate>
                                   <span>No Security roles have been set up.</span>
                               </EditItemTemplate>
                               <LayoutTemplate>
                                   <div class="row bginfo">
                                       <div class="col-sm-3 h4">Action</div>
                                       <div class="col-sm-3 h4">Role</div>
                                       <div class="col-sm-6 h4">Member</div>
                                   </div>
                                   <div runat="server" id="itemPlaceholder"></div>
                               </LayoutTemplate>
                               <ItemTemplate>
                                   <div class="row">
                                       <div class="col-sm-3">
                                           <asp:LinkButton ID="DeleteButton" runat="server"
                                               text="Delete" CommandName="Delete"></asp:LinkButton>
                                       </div>
                                       <div class="col-sm-3">
                                           <%# Item.RoleName %>
                                       </div>
                                       <div class="col-sm-6">
                                           <asp:Repeater ID="RoleUserRepeater" runat="server"
                                               DataSource="<%# Item.UserNames %>"
                                               ItemType="System.String">
                                               <ItemTemplate>
                                                   <%# Item %>
                                               </ItemTemplate>
                                           </asp:Repeater>
                                       </div>
                                   </div>
                               </ItemTemplate>
                               <InsertItemTemplate>
                                   <div class="row">
                                       <div class="col-sm-3">
                                           <asp:LinkButton ID="InsertButton" runat="server"
                                               Text="Insert" CommandName="Insert"></asp:LinkButton>
                                            <asp:LinkButton ID="CancelButton" runat="server"
                                               Text="Cancel" CommandName="Cancel"></asp:LinkButton>
                                       </div>
                                       <div class="col-sm-3">
                                           <asp:TextBox ID="RoleNameTextBox" runat="server"
                                               Text="<%# BindItem.RoleName %>" 
                                               placeholder="Role Name"></asp:TextBox>
                                       </div>
                                   </div>
                               </InsertItemTemplate>
                           </asp:ListView>
                            <asp:ObjectDataSource ID="RoleODS" runat="server"
                                OldValuesParameterFormatString="original_{0}"
                                SelectMethod="ListAllRoles"
                                DeleteMethod="DeleteRole" 
                                InsertMethod="AddRole"
                                TypeName="ChinookSystem.BLL.Security.RoleManager" 
                                DataObjectTypeName="Chinook.Data.Entities.Security.RoleProfile"
                                OnDeleted="CheckForException"
                                OnInserted="CheckForException"
                                OnSelected="CheckForException">
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </asp:UpdatePanel>   
                </div>
           </div>
        </div>
    </div>
</asp:Content>

