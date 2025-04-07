<%@ Page Title="Student Schedule" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="OrderSuccessfulPage.aspx.cs" 
    Inherits="MyScheduleWebsite.OrderSuccessfulPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background-color: #ffffff;
        margin: 0;
        padding: 0;
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 100vh;
    }

    .schedule-container {
        width: 80%;
        max-width: 800px;
        margin: 20px auto;
    }

    .schedule-card {
        background-color: #ffffff;
        padding: 20px;
        border: 1px solid #ffffff;
        border-radius: 8px;
    }

    .header {
        text-align: center;
        margin-bottom: 20px;
    }

    h2 {
        color: #3595cd;
        font-size: 1.8rem;
        font-weight: 700;
        margin-bottom: 10px;
    }

    .success-message {
        text-align: center;
        color: #3595cd;
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
        font-size: 1rem;
    }

    .info-value {
        font-size: 1rem;
        color: #000000; 
        font-weight: 600;
    }

    .schedule-table {
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
    }

    .action-buttons {
        display: flex;
        justify-content: center;
        margin-top: 20px;
        gap: 15px;
    }

    .btn {
        padding: 10px 24px;
        border: none;
        border-radius: 6px;
        font-size: 1rem;
        font-weight: 500;
        color: white;
        background-color: #3595cd;
        cursor: pointer;
        transition: background-color 0.3s ease;
    }

    .btn-cancel {
        background-color: #b2bcbd;
    }

    .btn-cancel:hover {
        background-color: #c82333;
    }

    .btn:hover {
        background-color: #afb6bc;
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
</style>

<div class="schedule-container">
    <div class="schedule-card">
        <div class="header">
            <h2>Your Schedule has been Created Successfully!</h2>
        </div>

        <div class="success-message">Your Schedule Details</div>
        
        <div class="student-info">
            <div class="info-row">
                <div class="info-label">Student Name:</div>
                <div class="info-value"><asp:Label ID="lblStudentName" runat="server" Text="Ahmed
Al-Otaibi"></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">University ID:</div>
                <div class="info-value"><asp:Label ID="lblUniID" runat="server" Text="123456789"></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">Major:</div>
                <div class="info-value"><asp:Label ID="lblMajor" runat="server" Text="Computer Science"></asp:Label></div>
            </div>
            <div class="info-row">
                <div class="info-label">Total Credits:</div>
                <div class="info-value"><asp:Label ID="lblTotalCredits" runat="server" Text="15"></asp:Label></div>
            </div>
        </div>
        
        <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="False" CssClass="schedule-table"
            OnRowDataBound="gvSchedule_RowDataBound" AllowSorting="True" OnSorting="gvSchedule_Sorting">
            <Columns>
                <asp:BoundField DataField="CourseCode" HeaderText="Course Code" SortExpression="CourseCode" />
                <asp:BoundField DataField="CourseName" HeaderText="Course Name" SortExpression="CourseName" />
                <asp:BoundField DataField="SectionNumber" HeaderText="Section No." SortExpression="SectionNumber" />
                <asp:BoundField DataField="Credits" HeaderText="Credits" SortExpression="Credits" />
                <asp:BoundField DataField="Schedule" HeaderText="Schedule" SortExpression="Schedule" />
                <asp:BoundField DataField="Instructor" HeaderText="Instructor" SortExpression="Instructor" />
            </Columns>
        </asp:GridView>
        
        <div class="action-buttons">
            <asp:Button ID="btnExport" runat="server" Text="Export to PDF" 
                CssClass="btn" OnClick="btnExport_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel Schedule" 
                CssClass="btn btn-cancel" OnClick="btnCancel_Click" />
        </div>
    </div>
</div>

</asp:Content>