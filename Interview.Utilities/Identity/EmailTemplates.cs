using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Interview.Utilities.Identity
{
    public static class EmailInterviews
    {
        
        public static string GetVerificationCodeEmailBody(string verificationCode,int VerificationCodeKeyTime)
        {
                            return $@"
                <!DOCTYPE html>
                <html lang='ar'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            overflow: hidden;
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 20px;
                            text-align: center;
                        }}
                        .content {{
                            padding: 20px;
                            text-align: right; /* Align text to the right for Arabic */
                        }}
                        .code {{
                            display: inline-block;
                            font-size: 24px;
                            font-weight: bold;
                            color: #007bff;
                            padding: 10px 20px;
                            border: 2px solid #007bff;
                            border-radius: 5px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            padding: 10px;
                            text-align: center;
                            font-size: 12px;
                            color: #999999;
                            background-color: #f9f9f9;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>تحقق من حسابك</h1>
                        </div>
                        <div class='content'>
                            <p>مرحبًا،</p>
                            <p>تم طلب رمز التحقق لحسابك. استخدم الرمز أدناه لإكمال عملية التحقق:</p>
                            <p>رمز التحقق ستنتهي صلاحيته بعد {VerificationCodeKeyTime} دقيقة من الآن.</p>
                            <div class='code'>{verificationCode}</div>
                            <p>إذا لم تكن قد طلبت هذا، يرجى تجاهل هذه الرسالة.</p>
                            <p>شكرًا لك!</p>
                        </div>
                        <div class='footer'>
                            <p>حقوق النشر © {DateTime.Now.Year} شركتنا. جميع الحقوق محفوظة.</p>
                        </div>
                    </div>
                </body>
                </html>
                ";
        }

        public static string GetEmailVerificationEmailBody(string callbackUrl)
        {

            return $@"
    <!DOCTYPE html>
    <html lang='ar'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            body {{
                font-family: 'Arial', sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}
            .container {{
                max-width: 600px;
                margin: 20px auto;
                background-color: #ffffff;
                border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                overflow: hidden;
            }}
            .header {{
                background-color: #db4b4e;
                color: #ffffff;
                padding: 25px;
                text-align: center;
                font-size: 24px;
                font-weight: bold;
            }}
            .content {{
                padding: 25px;
                text-align: right;
                font-size: 18px;
                line-height: 1.6;
            }}
            .button-container {{
                text-align: center; /* توسيط الزر */
                margin: 20px 0;
            }}
            .button {{
                display: inline-block;
                font-size: 20px;
                font-weight: bold;
                color: #db4b4e !important; /* لون الخط */
                background-color: #ffffff;
                padding: 12px 25px;
                border: 2px solid #db4b4e;
                border-radius: 5px;
                text-decoration: none; /* إزالة التسطير */
            }}
            .button:hover {{
                background-color: #db4b4e;
                color: #ffffff !important;
                transition: 0.3s;
            }}
            .footer {{
                padding: 15px;
                text-align: center;
                font-size: 14px;
                color: #ffffff;
                background-color: #db4b4e;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                تفعيل حسابك
            </div>
            <div class='content'>
                <p>مرحبًا،</p>
                <p>شكرًا لتسجيلك معنا. يرجى النقر على الزر أدناه لتفعيل حسابك:</p>
                <div class='button-container'>
                    <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' class='button'>تفعيل الحساب</a>
                </div>
                <p>إذا لم تكن قد طلبت هذا، يرجى تجاهل هذه الرسالة.</p>
                <p>شكرًا لك!</p>
            </div>
            <div class='footer'>
                <p>حقوق النشر © {DateTime.Now.Year} شركتنا. جميع الحقوق محفوظة.</p>
            </div>
        </div>
    </body>
    </html>
";



        }

    }
}
