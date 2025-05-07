<%@ Page Title="FAQ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FAQ.aspx.cs" Inherits="MyScheduleWebsite.FAQ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <style>
        .faq-header {
            background: linear-gradient(135deg, #6a11cb 0%, #2575fc 100%);
            color: white;
            padding: 60px 20px;
            text-align: center;
            margin-bottom: 40px;
            border-radius: 0 0 20px 20px;
        }

        .faq-title {
            font-size: 2.8rem;
            font-weight: 700;
            margin-bottom: 15px;
        }

        .faq-section {
            max-width: 1000px;
            margin: 0 auto 50px;
            padding: 0 20px;
        }

        .faq-card {
            background: white;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            padding: 30px;
            margin-bottom: 30px;
        }

        .faq-item {
            margin-bottom: 20px;
        }

        .faq-question {
            font-weight: bold;
            color: #6a11cb;
        }

        .faq-answer {
            margin-top: 5px;
            color: #333;
        }

        @media (max-width: 768px) {
            .faq-title {
                font-size: 2rem;
            }
        }
    </style>

    <div class="faq-header">
        <h1 class="faq-title">Frequently Asked Questions</h1>
    </div>

    <div class="faq-section">
        <div class="faq-card">
            <div class="faq-item">
                <div class="faq-question">1. How do I edit my profile information?</div>
                <div class="faq-answer">Go to the Edit Info page and update your details.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">2. I forgot my password. What should I do?</div>
                <div class="faq-answer">Click on “Forgot Password” on the login page and follow the instructions.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">3. Can I change my registered email?</div>
                <div class="faq-answer">Yes. Go to Edit Info and update your email address.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">4. What is the Student Progress page?</div>
                <div class="faq-answer">It shows your completed and remaining subjects based on your major and level.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">5. Who can see my profile info?</div>
                <div class="faq-answer">Only you and system admins can view or edit your information.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">6. What browsers are supported?</div>
                <div class="faq-answer">Chrome, Firefox, and Edge are fully supported.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">7. How do I logout?</div>
                <div class="faq-answer">Click the "Logout" link from the top navigation menu.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">8. Can faculty view student progress?</div>
                <div class="faq-answer">Yes, faculty with permission can view relevant student info.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">9. Can department heads change their password?</div>
                <div class="faq-answer">Yes, from the Edit Info page under their profile section.</div>
            </div>

            <div class="faq-item">
                <div class="faq-question">10. Who do I contact for help?</div>
                <div class="faq-answer">Please contact IT support or your system admin.</div>
            </div>
        </div>
    </div>
</asp:Content>
