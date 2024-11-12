<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="MyScheduleWebsite.StudentPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="main-container">
        <div class="info">
            <asp:Label ID="lblGreeting" runat="server" Text="Hello, name"></asp:Label>
            <div class="label-center">
                <asp:Label ID="lblCurrentLevel" runat="server" Text="Current Level: 4"></asp:Label>
            </div>
            <div class="hours-taken-container">
                <asp:Label ID="lblHoursTaken" runat="server" Text="Compulsory Hours Taken: 38 of 156"></asp:Label>
                <asp:Label ID="lblElectiveUniversityHoursTaken" runat="server" Text="Elective University Hours Taken: 2 of 4"></asp:Label>
                <asp:Label ID="lblElectiveCollegeHoursTaken" runat="server" Text="Elective College Hours Taken: 0 of 12"></asp:Label>
            </div>
        </div>

        <div class="status-container">
            <div class="status-bar">
                <div id="progress-bar" class="progress-bar-fill"></div>
            </div>
            <label id="hours-chosen-label">Hours chosen: 0</label>
        </div>

        <div class="choose-labels">
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose1" runat="server" CssClass="labels" Text="Choose your desired Compulsory subjects:"></asp:Label>
            </div>
            <div class="lbl-output">
                <asp:Label ID="lblOutput" runat="server" CssClass="labels" Text=""></asp:Label>
            </div>
        </div>

        <div id="subjectsContainer" class="subjects-container" runat="server">
            <div class="subjects-row">
                <asp:Label ID="Label1" runat="server" CssClass="labels" Text="Level 1"></asp:Label>
                <div class="subject taken" id="2004111-2" onclick="SubjectClicked(this)"><span>Islamic Culture</span></div>
                <div class="subject taken" id="202126-3" onclick="SubjectClicked(this)"><span>Fundamentals of Mathematics</span></div>
                <div class="subject taken" id="501110-2" onclick="SubjectClicked(this)"><span>Introduction to Problem Solving</span></div>
                <div class="subject taken" id="501112-2" onclick="SubjectClicked(this)"><span>Computer Skills</span></div>
                <div class="subject taken" id="990311-2" onclick="SubjectClicked(this)"><span>University Study Skills</span></div>
                <div class="subject taken" id="999805-2" onclick="SubjectClicked(this)"><span>Intensive English Intensive (1)</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label2" runat="server" CssClass="labels" Text="Level 2"></asp:Label>
                <div class="subject taken" id="105115-2" onclick="SubjectClicked(this)"><span>History of the Kingdom</span></div>
                <div class="subject taken" id="2011211-3" onclick="SubjectClicked(this)"><span>General Biology</span></div>
                <div class="subject taken" id="204124-2" onclick="SubjectClicked(this)"><span>General Chemistry</span></div>
                <div class="subject taken" id="501125-2" onclick="SubjectClicked(this)"><span>Scientific Computing</span></div>
                <div class="subject taken" id="503121-1" onclick="SubjectClicked(this)"><span>Computer Aided Drawing</span></div>
                <div class="subject taken" id="990211-2" onclick="SubjectClicked(this)"><span>Arabic Language Skills</span></div>
                <div class="subject taken" id="999806-2" onclick="SubjectClicked(this)"><span>Intensive English Intensive (2)</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label3" runat="server" CssClass="labels" Text="Level 3"></asp:Label>
                <div class="subject unoffered" id="202261-3" onclick="SubjectClicked(this)"><span>Calculus (1)</span></div>
                <div class="subject taken" id="203206-4" onclick="SubjectClicked(this)"><span>Physics (1)</span></div>
                <div class="subject taken" id="501215-3" onclick="SubjectClicked(this)"><span>Discrete Structures</span></div>
                <div class="subject available" id="501220-3" onclick="SubjectClicked(this)"><span>Computer Programming (1)</span></div>
                <div class="subject taken" id="999811-2" onclick="SubjectClicked(this)"><span>English for Specific Purpose (1)</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label4" runat="server" CssClass="labels" Text="Level 4"></asp:Label>
                <div class="subject available" id="2004112-2" onclick="SubjectClicked(this)"><span>Islamic Culture (Morals &amp; Values)</span></div>
                <div class="subject unavailable" id="202263-3" onclick="SubjectClicked(this)"><span>Calculus (2)</span></div>
                <div class="subject unoffered" id="203207-4" onclick="SubjectClicked(this)"><span>Physics (2)</span></div>
                <div class="subject unavailable" id="501222-3" onclick="SubjectClicked(this)"><span>Computer Programming (2)</span></div>
                <div class="subject available" id="503221-4" onclick="SubjectClicked(this)"><span>Digital Logic Design</span></div>
                <div class="subject available" id="999808-2" onclick="SubjectClicked(this)"><span>English for Specific Purpose (2)</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label5" runat="server" CssClass="labels" Text="Level 5"></asp:Label>
                <div class="subject unavailable" id="202262-3" onclick="SubjectClicked(this)"><span>Linear Algebra</span></div>
                <div class="subject unavailable" id="202364-3" onclick="SubjectClicked(this)"><span>Probability and Statistics</span></div>
                <div class="subject unavailable" id="501323-3" onclick="SubjectClicked(this)"><span>Objected-oriented Programming</span></div>
                <div class="subject unavailable" id="501324-3" onclick="SubjectClicked(this)"><span>Data Structures</span></div>
                <div class="subject unavailable" id="501326-3" onclick="SubjectClicked(this)"><span>MicroProcessors &amp; Assembly Language</span></div>
                <div class="subject unavailable" id="501352-3" onclick="SubjectClicked(this)"><span>Introduction to Operating</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label6" runat="server" CssClass="labels" Text="Level 6"></asp:Label>
                <div class="subject unavailable" id="2004313-2" onclick="SubjectClicked(this)"><span>Islamic culture (Social System in Islam)</span></div>
                <div class="subject unavailable" id="202368-3" onclick="SubjectClicked(this)"><span>Operations Research</span></div>
                <div class="subject unavailable" id="501343-3" onclick="SubjectClicked(this)"><span>Software Engineering</span></div>
                <div class="subject unavailable" id="502372-3" onclick="SubjectClicked(this)"><span>Fundamentals of Database</span></div>
                <div class="subject unavailable" id="503323-3" onclick="SubjectClicked(this)"><span>Computer Architecture</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label7" runat="server" CssClass="labels" Text="Level 7"></asp:Label>
                <div class="subject unavailable" id="2004414-2" onclick="SubjectClicked(this)"><span>Islamic Culture (Human Rights)</span></div>
                <div class="subject unavailable" id="501435-3" onclick="SubjectClicked(this)"><span>Analysis and Design of Algorithms</span></div>
                <div class="subject unavailable" id="501446-3" onclick="SubjectClicked(this)"><span>Advanced Software Engineering</span></div>
                <div class="subject unavailable" id="501453-3" onclick="SubjectClicked(this)"><span>Operating Systems</span></div>
                <div class="subject unavailable" id="501481-3" onclick="SubjectClicked(this)"><span>Artificial Intelligence</span></div>
                <div class="subject unavailable" id="503410-3" onclick="SubjectClicked(this)"><span>Data Communication</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label8" runat="server" CssClass="labels" Text="Level 8"></asp:Label>
                <div class="subject unavailable" id="500321-2" onclick="SubjectClicked(this)"><span>Professional Ethics</span></div>
                <div class="subject unavailable" id="501427-3" onclick="SubjectClicked(this)"><span>Programming Paradigms</span></div>
                <div class="subject unavailable" id="501461-3" onclick="SubjectClicked(this)"><span>Internet Technologies</span></div>
                <div class="subject unavailable" id="501472-3" onclick="SubjectClicked(this)"><span>Computer Graphics</span></div>
                <div class="subject unavailable" id="503442-3" onclick="SubjectClicked(this)"><span>Computer Networks</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label9" runat="server" CssClass="labels" Text="Level 9"></asp:Label>
                <div class="subject unavailable" id="202463-3" onclick="SubjectClicked(this)"><span>Advanced Mathematics</span></div>
                <div class="subject unavailable" id="501554-3" onclick="SubjectClicked(this)"><span>Distributed Systems</span></div>
                <div class="subject unavailable" id="501598-3" onclick="SubjectClicked(this)"><span>Capstone Project (1)</span></div>
                <div class="subject unavailable" id="501588-2" onclick="SubjectClicked(this)"><span>Field Experience</span></div>
            </div>
            <div class="subjects-row">
                <asp:Label ID="Label10" runat="server" CssClass="labels" Text="Level 10"></asp:Label>
                <div class="subject unavailable" id="501599-3" onclick="SubjectClicked(this)"><span>Capstone Project (2)</span></div>
                <div class="subject unavailable" id="502459-3" onclick="SubjectClicked(this)"><span>Computer Systems Security</span></div>
            </div>
        </div>

        <div class="choose-labels">
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose2" runat="server" CssClass="labels" Text="Choose your desired University Elective subjects:"></asp:Label>
            </div>
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose3" runat="server" CssClass="labels" Text="Choose your desired College Elective subjects:"></asp:Label>
            </div>
        </div>

        <div class="elective-container">
            <div id="electiveUniversity" class="elective-subjects-container">
                <div class="subjects-row">
                    <asp:Label ID="Label11" runat="server" CssClass="labels" Text="Level 3"></asp:Label>
                    <div class="subject taken" id="990113-2" onclick="SubjectClicked(this)"><span>Health Education</span></div>
                    <div class="subject unavailable" id="990114-2" onclick="SubjectClicked(this)"><span>Saudi Women Empowerment</span></div>
                    <div class="subject unavailable" id="990312-2" onclick="SubjectClicked(this)"><span>Innovation and Entrepreneurship</span></div>
                    <div class="subject unavailable" id="990412-2" onclick="SubjectClicked(this)"><span>Digital Citizenship</span></div>
                </div>
                <div class="subjects-row">
                    <asp:Label ID="Label12" runat="server" CssClass="labels" Text="Level 6"></asp:Label>
                    <div class="subject unavailable" id="990314-2" onclick="SubjectClicked(this)"><span>French Language</span></div>
                    <div class="subject unavailable" id="990315-2" onclick="SubjectClicked(this)"><span>Chinese Language</span></div>
                    <div class="subject unavailable" id="999809-2" onclick="SubjectClicked(this)"><span>Presentation Skills</span></div>
                    <div class="subject unavailable" id="999814-2" onclick="SubjectClicked(this)"><span>IELTS Preparation</span></div>
                    <div class="subject unavailable" id="999815-2" onclick="SubjectClicked(this)"><span>Academic Writing</span></div>
                    <div class="subject unavailable" id="999821-2" onclick="SubjectClicked(this)"><span>English and the 21st Century Skills</span></div>
                </div>
            </div>

            <div id="electiveCollege" class="elective-subjects-container">
                <div class="subjects-row">
                    <asp:Label ID="Label13" runat="server" CssClass="labels" Text="Level 8"></asp:Label>
                    <div class="subject unavailable" id="501471-3" onclick="SubjectClicked(this)"><span>Modeling and Simulation</span></div>
                    <div class="subject unavailable" id="501573-3" onclick="SubjectClicked(this)"><span>Image Processing</span></div>
                    <div class="subject unavailable" id="501585-3" onclick="SubjectClicked(this)"><span>Expert Systems</span></div>
                    <div class="subject unavailable" id="502478-3" onclick="SubjectClicked(this)"><span>Data Warehouse</span></div>
                    <div class="subject unavailable" id="502536-3" onclick="SubjectClicked(this)"><span>Human Computer Interaction</span></div>
                </div>
                <div class="subjects-row">
                    <asp:Label ID="Label14" runat="server" CssClass="labels" Text="Level 9"></asp:Label>
                    <div class="subject unavailable" id="501454-3" onclick="SubjectClicked(this)"><span>Compiler Design</span></div>
                    <div class="subject unavailable" id="501496-3" onclick="SubjectClicked(this)"><span>Special Topics in Computer Systems</span></div>
                    <div class="subject unavailable" id="501528-3" onclick="SubjectClicked(this)"><span>Game Design and Programming</span></div>
                    <div class="subject unavailable" id="501536-3" onclick="SubjectClicked(this)"><span>Parallel and Distributed Algorithms</span></div>
                    <div class="subject unavailable" id="503509-3" onclick="SubjectClicked(this)"><span>Embedded Systems</span></div>
                    <div class="subject unavailable" id="503547-3" onclick="SubjectClicked(this)"><span>Network Programming</span></div>
                </div>
                <div class="subjects-row">
                    <asp:Label ID="Label15" runat="server" CssClass="labels" Text="Level 10"></asp:Label>
                    <div class="subject unavailable" id="501424-3" onclick="SubjectClicked(this)"><span>Computer Vision</span></div>
                    <div class="subject unavailable" id="501513-3" onclick="SubjectClicked(this)"><span>Cryptology</span></div>
                    <div class="subject unavailable" id="501562-3" onclick="SubjectClicked(this)"><span>Pervasive Computing</span></div>
                    <div class="subject unavailable" id="501570-3" onclick="SubjectClicked(this)"><span>Selected Topics in Artificial Intelligence</span></div>
                    <div class="subject unavailable" id="501575-3" onclick="SubjectClicked(this)"><span>Multimedia Systems</span></div>
                    <div class="subject unavailable" id="501582-3" onclick="SubjectClicked(this)"><span>Neural Networks</span></div>
                    <div class="subject unavailable" id="501583-3" onclick="SubjectClicked(this)"><span>Pattern Recognition</span></div>
                    <div class="subject unavailable" id="501592-3" onclick="SubjectClicked(this)"><span>Special Topics in Programming Languages</span></div>
                    <div class="subject unavailable" id="501593-3" onclick="SubjectClicked(this)"><span>Special Topics in Algorithms</span></div>
                    <div class="subject unavailable" id="501595-3" onclick="SubjectClicked(this)"><span>Special Topics in Database Systems</span></div>
                    <div class="subject unavailable" id="502571-3" onclick="SubjectClicked(this)"><span>Data Mining</span></div>
                    <div class="subject unavailable" id="503527-3" onclick="SubjectClicked(this)"><span>Mobile Computing</span></div>
                    <div class="subject unavailable" id="503538-3" onclick="SubjectClicked(this)"><span>Analysis of Computer Systems Performance</span></div>
                    <div class="subject unavailable" id="503578-3" onclick="SubjectClicked(this)"><span>Robotics</span></div>
                </div>
            </div>

        </div>

        <%--<div class="subjects-container" id="subjectsContainer" runat="server">
            <!-- Subjects will be dynamically added here -->
        </div>--%>


        <asp:Label ID="lblSelectedSubjects" runat="server" Text=""></asp:Label>
        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Default.aspx" CssClass="custom-button"/>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="custom-button"/>
        </div>

        <div class="help-center">
            <div class="help-button" onclick="toggleHelpCenter()">?</div>
            <div class="help-content">
                <div class="email-options">
                    <button class="email-option">Email Admin</button>
                    <button class="email-option">Email Faculty</button>
                </div>
                <input type="text" placeholder="Email Title" class="email-title">
                <textarea placeholder="Type your message here..." class="email-message"></textarea>
                <button class="send-button">Send</button>
            </div>
        </div>


    </div>

    <script>
        var selectedSubjects = [];
        var totalHours = 0;

        function updateProgressBar() {
            var progressBar = document.getElementById('progress-bar');
            var hoursLabel = document.getElementById('hours-chosen-label');
            var lblOutput = document.getElementById('<%= lblOutput.ClientID %>');

            hoursLabel.innerText = "Hours chosen: " + totalHours;

            var progressWidth = (totalHours / 20) * 100;
            progressBar.style.width = progressWidth + '%';

            if (totalHours < 12) {
                progressBar.style.backgroundColor = 'red';
                lblOutput.innerText = "";
            } else if (totalHours >= 12 && totalHours < 20) {
                progressBar.style.backgroundColor = 'green';
                lblOutput.innerText = "";
            } else if (totalHours >= 20) {
                progressBar.style.backgroundColor = 'green';
                displayAlert("You have reached the maximum limit of 20 hours.");
            }
        }

        function SubjectClicked(element) {
            var subjectCode = element.id;
            var lblOutput = document.getElementById('<%= lblOutput.ClientID %>');

            lblOutput.innerText = "";

            if (!element.classList.contains('available') && !element.classList.contains('unoffered')) {
                displayAlert("You cannot choose this subject.");
                return;
            }

            if (selectedSubjects.includes(subjectCode)) {
                selectedSubjects = selectedSubjects.filter(item => item !== subjectCode);
                totalHours -= 3;
                element.classList.remove('selected');
            } else if (totalHours + 3 <= 20) {
                selectedSubjects.push(subjectCode);
                totalHours += 3;
                element.classList.add('selected');
            } else {
                displayAlert("You can't select more than 20 hours.");
                return;
            }

            updateProgressBar();
        }

        function displayAlert(message) {
            var lblOutput = document.getElementById('<%= lblOutput.ClientID %>');
            lblOutput.innerText = message;
            lblOutput.style.color = 'red';
        }

        function toggleHelpCenter() {
            const content = document.querySelector('.help-content');
            content.classList.toggle('active');
        }
    </script>

</asp:Content>
