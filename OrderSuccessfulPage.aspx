﻿<%@ Page Title="Student Schedule" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="OrderSuccessfulPage.aspx.cs" 
    Inherits="MyScheduleWebsite.OrderSuccessfulPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .schedule-container {
        width: 100%;
        margin: 20px auto;
    }

    .schedule-card {
        background-color: #f7f7f7;
        padding: 20px;
        border-radius: 8px;
    }

    .header {
        text-align: center;
        margin-bottom: 20px;
    }

    h2 {
        /*color: #3595cd;*/
        font-size: 2rem;
        font-weight: 700;
        margin-bottom: 10px;
    }

    .success-message {
        text-align: center;
        /*color: #3595cd;*/
        font-size: 1.6rem;
        font-weight: 700;
        margin-bottom: 1em;
    }

    .student-info {
        margin-bottom: 20px;
        padding: 15px;
        background-color: #f5f5f5;
        border-radius: 8px;
        border: 1px solid #e0e0e0;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .info-row {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .info-label {
        font-weight: 600;
        color: #3595cd;
        font-size: 1.3rem;
    }

    .info-value {
        font-size: 1.3rem;
        color: #000000; 
        font-weight: 600;
    }

    /*.schedule-table {
        width: 100%;
        border-collapse: collapse;
        margin: 20px 0;
        border: 1px solid #e0e0e0;
    }

    .schedule-table th {
        background-color: #b2bcbd;
        color: white; 
        padding: 12px;
        text-align: center;
        font-weight: 500;
        font-size: 1rem;
        border: 1px solid #e0e0e0;
    }

    .schedule-table td {
        padding: 10px;
        text-align: center;
        border: 1px solid #e0e0e0;
        font-size: 1rem;
        color: #000000; 
    }

    .schedule-table tr:nth-child(even) {
        background-color: #f9f9f9;
    }*/

    .action-buttons {
        display: flex;
        justify-content: center;
        margin-top: 50px;
        gap: 30px;
    }

    @media (max-width: 768px) {
        .student-info {
            flex-direction: column;
            align-items: flex-start;
        }

        .schedule-card {
            padding: 15px;
            width: 90%;
        }

        h2 {
            font-size: 1.4rem;
        }

        .success-message {
            font-size: 1.2rem;
        }
        
        .action-buttons {
            flex-direction: column;
            align-items: center;
        }
        
        .btn {
            width: 100%;
            margin-bottom: 10px;
        }

    }

        .schedule-table
  
    {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;
        margin: 1.5rem 0;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
        border-radius: 8px;
        text-align:center;
        overflow: hidden;
    }

    .schedule-table th
    
    {
        background-color: #f8f9fa;
        padding: 0.75rem 1rem;
        font-weight: 600;
        letter-spacing: 0.05em;
        color: #343a40;
        border-bottom: 2px solid #dee2e6;
        text-align: center;
    }


    .schedule-table tbody td
    
    {
        padding: 0.75rem 1rem;
        color: #495057;
        border-bottom: 1px solid #e9ecef;
        text-align: center;
    }


    .schedule-table tbody tr:nth-child(even)
     {
        background-color: #fcfcfc;
    }


    .schedule-table tbody tr:hover
     {
        background-color: #f1f3f5;
    }
</style>

<div class="schedule-container">
    <div class="schedule-card">
        <div class="header">
            <h2>Your Schedule has been Created Successfully!</h2>
        </div>
        <br /><br />
        <div class="success-message">Schedule Details:</div>
        
        <div class="student-info">
            <div class="info-row">
                <div class="info-label">Student Name:</div>
                <div class="info-value"><asp:Label ID="lblStudentName" runat="server" Text=""></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">University ID:</div>
                <div class="info-value"><asp:Label ID="lblUniID" runat="server" Text=""></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">Major:</div>
                <div class="info-value"><asp:Label ID="lblMajor" runat="server" Text=""></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">Total Credits:</div>
                <div class="info-value"><asp:Label ID="lblTotalCredits" runat="server" Text=""></asp:Label></div>
            </div>
        </div>
        
        <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="False" CssClass="schedule-table"
            OnRowDataBound="gvSchedule_RowDataBound" AllowSorting="True" OnSorting="gvSchedule_Sorting">
            <Columns>
                <asp:BoundField DataField="SubjectCode" HeaderText="Subject Code" />
                <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" />
                <asp:BoundField DataField="Credits" HeaderText="Credits" />
                <asp:BoundField DataField="SectionNumber" HeaderText="Section No." />
                <asp:TemplateField HeaderText="Day">
                    <ItemTemplate>
                        <asp:Label ID="lblDay" runat="server" Text='<%# Eval("Day") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Start Time">
                    <ItemTemplate>
                        <asp:Label ID="lblStartTime" runat="server" Text='<%# Eval("StartTime") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Time">
                    <ItemTemplate>
                        <asp:Label ID="lblEndTime" runat="server" Text='<%# Eval("EndTime") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Location" HeaderText="Location" />
                <asp:BoundField DataField="Instructor" HeaderText="Instructor" />
            </Columns>
        </asp:GridView>
        
        <asp:Panel ID="pnlWishlist" runat="server" Visible="false">
            <div class="success-message" style="margin-top: 40px;">
                Wishlisted Subjects
            </div>
            <asp:GridView ID="gvWishlist" runat="server" AutoGenerateColumns="False" CssClass="schedule-table">
                <Columns>
                    <asp:BoundField DataField="SubjectCode" HeaderText="Subject Code" />
                    <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" />
                    <asp:BoundField DataField="RequestDate" HeaderText="Request Date" 
                                  DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:TemplateField HeaderText="Status Changed By">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# string.IsNullOrEmpty(Eval("StatusChangedBy").ToString()) ? "-" : Eval("StatusChangedBy") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status Changed Date">
                        <ItemTemplate>
                            <asp:Label runat="server" 
                                 Text='<%# Eval("StatusChangedDate") is DBNull ? "-" : Convert.ToDateTime(Eval("StatusChangedDate")).ToString("dd MMM yyyy") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>

        <div class="action-buttons">
           <%-- <asp:Button ID="btnExport" runat="server" Text="Export to PDF" 
                CssClass="btn-custom-style" OnClick="btnExport_Click" />--%>
            <asp:Button ID="btnCancel" runat="server" Text="Cancel Schedule" 
                CssClass="btn-custom-style no" OnClick="btnCancel_Click"
                OnClientClick="return confirm('Are you sure you want to cancel your order and delete all ordered subjects?');"/>
        </div>
    </div>
</div>

</asp:Content>