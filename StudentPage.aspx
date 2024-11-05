<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="MyScheduleWebsite.StudentPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="top-container center">
        <div class="top-container center">
            <div class="stepper">
                <div class="step active" id="step1">1</div>
                <div class="line"></div>
                <div class="step" id="step2">2</div>
                <div class="line"></div>
                <div class="step" id="step3">3</div>
                <div class="line"></div>
                <div class="step" id="step4">4</div>
            </div>
        </div>
    </div>
    <div class="main-container">
        <div class="info">
            <asp:Label ID="lblGreeting" runat="server" Text="Hello, name"></asp:Label>
            <asp:Label ID="lblCurrentLevel" runat="server" Text="Current Level:"></asp:Label>
            <asp:Label ID="lblHoursTaken" runat="server" Text="Hours Taken: TODO"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblChoose" runat="server" Text="Choose your desired compulsory subjects:"></asp:Label>
            <br />
            <br />
        </div>
        <div class="subjects-container" id="subjectsContainer" runat="server">
            <!-- Subjects will be dynamically added here -->
        </div>
        <asp:Label ID="lblSelectedSubjects" runat="server" Text=""></asp:Label>
        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Default.aspx" CssClass="custom-button" OnClick="btnBack_Click" />
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="custom-button" OnClick="btnNext_Click" />
        </div>

    </div>

    <script>
        var selectedSubjects = [];

        function SubjectClicked(element) {
            var subjectCode = element.id;

            if (selectedSubjects.includes(subjectCode)) {
                selectedSubjects = selectedSubjects.filter(item => item !== subjectCode);
                element.classList.remove('selected');
            } else {
                selectedSubjects.push(subjectCode);
                element.classList.add('selected');
            }

            document.getElementById('<%= lblSelectedSubjects.ClientID %>').innerText = selectedSubjects.join(', ');
        }
    </script>

</asp:Content>
