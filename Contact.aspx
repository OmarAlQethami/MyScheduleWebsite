<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Contact.aspx.cs" Inherits="MyScheduleWebsite.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <style>
        /* Contact Page Styles */
        .contact-header {
            background: linear-gradient(135deg, #6a11cb 0%, #2575fc 100%);
            color: white;
            padding: 60px 20px;
            text-align: center;
            margin-bottom: 40px;
            border-radius: 0 0 20px 20px;
        }
        
        .contact-title {
            font-size: 2.8rem;
            font-weight: 700;
            margin-bottom: 15px;
        }
        
        .contact-subtitle {
            font-size: 1.3rem;
            opacity: 0.9;
        }
        
        .contact-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 20px;
            display: flex;
            flex-wrap: wrap;
            gap: 30px;
        }
        
        .contact-info {
            flex: 1;
            min-width: 300px;
        }
        
        .contact-form {
            flex: 1;
            min-width: 300px;
        }
        
        .contact-card {
            background: white;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            padding: 30px;
            margin-bottom: 30px;
        }
        
        .contact-card h3 {
            color: #6a11cb;
            margin-bottom: 20px;
            font-size: 1.8rem;
            display: flex;
            align-items: center;
        }
        
        .contact-card h3 i {
            margin-right: 15px;
            font-size: 1.5rem;
        }
        
        .contact-item {
            display: flex;
            align-items: flex-start;
            margin-bottom: 20px;
        }
        
        .contact-icon {
            color: #2575fc;
            font-size: 1.3rem;
            margin-right: 15px;
            margin-top: 5px;
        }
        
        .form-group {
            margin-bottom: 20px;
        }
        
        .form-control {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 1rem;
        }
        
        textarea.form-control {
            min-height: 150px;
        }
        
        .btn-contact {
            background: #6a11cb;
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 50px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .btn-contact:hover {
            background: #520da1;
            transform: translateY(-2px);
        }
        
        /* Responsive Design */
        @media (max-width: 768px) {
            .contact-title {
                font-size: 2rem;
            }
            
            .contact-subtitle {
                font-size: 1.1rem;
            }
            
            .contact-container {
                flex-direction: column;
            }
        }
    </style>

    <div class="contact-header">
        <h1 class="contact-title">Contact Us</h1>
        <p class="contact-subtitle">We'd love to hear from you!</p>
    </div>

    <div class="contact-container">
        <div class="contact-info">
            <div class="contact-card">
                <h3><i class="fas fa-envelope-open-text"></i>Get In Touch</h3>
                
                <div class="contact-item">
                    <i class="fas fa-envelope contact-icon"></i>
                    <div>
                        <h4>Email</h4>
                        <p><a href="mailto:MyScheduleWebsite@gmail.com">MyScheduleWebsite@gmail.com</a></p>
                    </div>
                </div>
                
                <!-- تم حذف قسم Working Hours هنا -->
            </div>
            
            <div class="contact-card">
                <h3><i class="fas fa-share-alt"></i>Follow Us</h3>
                <div style="display: flex; gap: 15px; font-size: 1.5rem;">
                    <a href="#" style="color: #3b5998;"><i class="fab fa-facebook"></i></a>
                    <a href="#" style="color: #1da1f2;"><i class="fab fa-twitter"></i></a>
                    <a href="#" style="color: #0077b5;"><i class="fab fa-linkedin"></i></a>
                    <a href="#" style="color: #e4405f;"><i class="fab fa-instagram"></i></a>
                </div>
            </div>
        </div>
        
        <div class="contact-form">
            <div class="contact-card">
                <h3><i class="fas fa-paper-plane"></i>Send Us a Message</h3>
                
                <div class="form-group">
                    <label for="txtName">Your Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter your name"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label for="txtEmail">Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" TextMode="Email"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label for="txtSubject">Subject</label>
                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" placeholder="What's this about?"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label for="txtMessage">Message</label>
                    <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="Your message here..."></asp:TextBox>
                </div>
                
                <asp:Button ID="btnSubmit" runat="server" Text="Send Message" CssClass="btn-contact" />
            </div>
        </div>
    </div>

</asp:Content>