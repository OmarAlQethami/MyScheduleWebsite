<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="About.aspx.cs" Inherits="MyScheduleWebsite.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">
    <style>
        /* About Page Styles */
        .about-header {
            background: linear-gradient(135deg, #6a11cb 0%, #2575fc 100%);
            color: white;
            padding: 60px 20px;
            text-align: center;
            margin-bottom: 40px;
            border-radius: 0 0 20px 20px;
        }
        
        .about-title {
            font-size: 2.8rem;
            font-weight: 700;
            margin-bottom: 15px;
        }
        
        .about-subtitle {
            font-size: 1.3rem;
            opacity: 0.9;
        }
        
        .about-section {
            max-width: 1200px;
            margin: 0 auto 50px;
            padding: 0 20px;
        }
        
        .about-card {
            background: white;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
            padding: 30px;
            margin-bottom: 30px;
            transition: transform 0.3s ease;
        }
        
        .about-card:hover {
            transform: translateY(-5px);
        }
        
        .about-card h3 {
            color: #6a11cb;
            margin-bottom: 20px;
            font-size: 1.8rem;
        }
        
        .about-card h3 i {
            margin-right: 15px;
        }
        
        .feature-box {
            display: flex;
            align-items: flex-start;
            margin-bottom: 20px;
        }
        
        .feature-icon {
            color: #2575fc;
            font-size: 1.5rem;
            margin-right: 15px;
            margin-top: 5px;
        }
        
        .feature-content h4 {
            color: #333;
            margin-bottom: 5px;
            font-size: 1.2rem;
        }
        
        .btn-about {
            display: inline-block;
            padding: 12px 25px;
            background: #6a11cb;
            color: white;
            border-radius: 50px;
            text-decoration: none;
            font-weight: 600;
            transition: all 0.3s ease;
            border: none;
            cursor: pointer;
        }
        
        .btn-about:hover {
            background: #520da1;
            transform: translateY(-2px);
            color: white;
        }
        
        /* Why Choose Us Grid */
        .features-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-top: 30px;
        }
        
        .feature-card {
            background: #f9f9ff;
            padding: 25px;
            border-radius: 10px;
            text-align: center;
            border: 1px solid #eee;
        }
        
        .feature-card i {
            font-size: 2.5rem;
            color: #6a11cb;
            margin-bottom: 15px;
        }
        
        /* Responsive Design */
        @media (max-width: 768px) {
            .about-title {
                font-size: 2rem;
            }
            
            .about-subtitle {
                font-size: 1.1rem;
            }
            
            .about-card {
                padding: 20px;
            }
        }
    </style>

    <div class="about-header">
        <h1 class="about-title">About My Schedule</h1>
        <p class="about-subtitle">Empowering students to take control of their academic journey</p>
    </div>

    <div class="about-section">
        <div class="about-card">
            <h3><i class="fas fa-rocket"></i>Our Mission</h3>
            <p>At My Schedule, we're dedicated to revolutionizing how students manage their academic lives. Our platform provides intuitive tools for schedule planning, deadline tracking, and progress monitoring, helping you achieve academic success with less stress.</p>
            
            <div class="feature-box">
                <i class="fas fa-check-circle feature-icon"></i>
                <div class="feature-content">
                    <h4>Simplify Your Academic Life</h4>
                    <p>We eliminate the complexity of schedule management so you can focus on learning.</p>
                </div>
            </div>
            
            <div class="feature-box">
                <i class="fas fa-lightbulb feature-icon"></i>
                <div class="feature-content">
                    <h4>Smart Planning Tools</h4>
                    <p>Our intelligent algorithms help you create optimal schedules based on your preferences.</p>
                </div>
            </div>
        </div>
    </div>

    <div class="about-section">
        <div class="about-card">
            <h3><i class="fas fa-star"></i>Why Choose Us?</h3>
            <p>My Schedule stands out from other planning tools with these key features:</p>
            
            <div class="features-grid">
                <div class="feature-card">
                    <i class="fas fa-user-graduate"></i>
                    <h4>Student-Centric Design</h4>
                    <p>Built by students for students, with real understanding of academic challenges.</p>
                </div>
                
                <div class="feature-card">
                    <i class="fas fa-lock"></i>
                    <h4>Privacy First</h4>
                    <p>Your data is always secure and never shared with third parties.</p>
                </div>
                
                <div class="feature-card">
                    <i class="fas fa-sync-alt"></i>
                    <h4>Continuous Updates</h4>
                    <p>We regularly add new features based on student feedback.</p>
                </div>
                
                <div class="feature-card">
                    <i class="fas fa-headset"></i>
                    <h4>24/7 Support</h4>
                    <p>Our dedicated support team is always ready to help.</p>
                </div>
            </div>
        </div>
    </div>

</asp:Content>