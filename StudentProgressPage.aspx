<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentProgressPage.aspx.cs" Inherits="MyScheduleWebsite.StudentProgressPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/StudentStyles.css">

    <div class="main-container">
        <div class="info">
            
        </div>
        <div class="choose-labels">
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose1" runat="server" CssClass="labels" Text="Choose your previously taken subjects:"></asp:Label>
            </div>
            <div class="lbl-output">
                <asp:Label ID="lblOutput" runat="server" CssClass="labels" Text=""></asp:Label>
            </div>
        </div>
        <div id="subjectsContainer" class="subjects-container" runat="server">

        </div>
        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Default.aspx" CssClass="custom-button"/>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="custom-button"/>
        </div>
    </div>

    <script>
        var selectedSubjects = [];
        var totalHours = 0;

        function SubjectClicked(element) {
            var subjectCode = element.id;
            var lblOutput = document.getElementById('<%= lblOutput.ClientID %>');

            lblOutput.innerText = "";

            //if (!element.classList.contains('available') && !element.classList.contains('unoffered')) {
            //    displayAlert("You cannot choose this subject.");
            //    return;
            //}

            if (selectedSubjects.includes(subjectCode)) {
                selectedSubjects = selectedSubjects.filter(item => item !== subjectCode);
                totalHours -= 3;
                element.classList.remove('history-selected');
            } else if (totalHours + 3 <= 20) {
                selectedSubjects.push(subjectCode);
                totalHours += 3;
                element.classList.add('history-selected');
            } else {
                displayAlert("You can't select more than 20 hours.");
                return;
            }
        }

        function displayAlert(message) {
            var lblOutput = document.getElementById('<%= lblOutput.ClientID %>');
            lblOutput.innerText = message;
            lblOutput.style.color = 'red';
        }
    </script>

</asp:Content>
