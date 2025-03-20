# Muqabalati (مقابلتي)

"Muqabalati" is an innovative application designed to empower Arab youth by simulating realistic job interviews using Generative AI. The current version focuses on **voice-based interviews**, supporting various Arabic dialects (e.g., Egyptian, Syrian, Yemani, Sudanese, Saudi). It helps users improve their interview skills, boost confidence, and increase employability. Future updates will include text-based responses and video simulations.


## Problem
- **High Unemployment:** 30% of Arab youth are unemployed, with inadequate interview preparation being a significant contributing factor (World Bank, 2023).  
  Source: [Youth Unemployment Rate for the Arab World](https://fred.stlouisfed.org/series/SLUEM1524ZSARB).  
- **Lack of Interview Skills:** Many lack practical training and confidence, struggling with common questions like "Tell me about yourself" or "Why are you suitable for this role?".  
- **Scarcity of Free Tools Supporting Arabic Dialects:** Most available tools are in foreign languages or do not support local dialects (Egyptian, Syrian, Yemani, Sudanese, Saudi), making them less effective.  
- **High Costs of Available Resources:** Existing tools or courses for interview preparation are often expensive, making them inaccessible to youth in rural areas or with limited income.  
- **Missed Job Opportunities Due to Poor Interview Understanding:** Many lose job opportunities because they don’t know how to present themselves professionally or handle interview questions confidently, negatively impacting employer impressions.


## Solution
"Muqabalati" provides an interactive platform where users can:  
- Choose a job category (e.g., marketing, tech) and their preferred Arabic dialect.  
- Practice with AI-generated interview questions delivered via voice (Text-to-Speech).  
- Respond using voice (Speech-to-Text).  
- Receive instant feedback and personalized tips to enhance their performance.  

The current version is voice-only (both questions and responses are voice-based). Text-based responses and video avatars are planned for future releases.


## Key Features
- **Voice-Based Simulations:** Realistic interview questions delivered in local Arabic dialects using Text-to-Speech, with responses recorded via Speech-to-Text.  
- **Real-Time Feedback:** AI evaluates voice responses and provides improvement suggestions.  
- **Accessibility:** Free core features, with premium options planned.  
- **Customizable:** Supports multiple dialects and job types.


## Target Audience
- **Primary:** Arab youth (18-35) seeking employment (~100M potential users across the Arab world).  
- **Secondary:** Universities, recruitment firms, and government programs tackling unemployment.


## Tech Stack
- **Frontend:** HTML5, CSS3, JavaScript, Bootstrap.  
- **Backend:** ASP.NET Core(Web API, MVC), C#, Entity Framework Core.  
- **Database:** Microsoft SQL Server. 
- **AI:** Google Gemini for question generation and feedback analysis.
- **Voice Processing:** Web Speech API for Speech-to-Text and Text-to-Speech converting.  
- **Design:** Figma for UI/UX prototyping.


## Prerequisites

To run this application, you will need the following:

- **.NET SDK**: Version 9.0 or later
- **SQL Server**: A local or remote SQL Server instance


## Getting Started

Follow these steps to set up and run the application:

### 1. Clone the Repository

Clone the repository to your local machine using Git:

```bash
git clone [https://github.com/arammtech/Muqabalati.git]
```

### 2. Open the Project

Open the project in your preferred IDE. Visual Studio 2022 is recommended for full ASP.NET Core and EF Core support. Simply open the solution file (.sln) in Visual Studio.

### 3. Configure the Database

- Open the `appsettings.json` file located in the root of the project.
- Update the connection string under `"ConnectionStrings": { "DefaultConnection": "Your_SQL_Server_Connection_String" }` to match your SQL Server configuration.

### 4. Build the Application

Navigate to the project directory and build the application:

```bash
dotnet build
```

### 5. Run the Application

Simply run the application to initialize the database and start the server. The application is configured to check for any pending migrations on startup and apply them automatically if needed.

```bash
dotnet run
```


## Usage
1. Launch the app (once developed).  
2. Select your dialect and job category.  
3. Listen to AI-generated questions delivered via voice.  
4. Respond using your voice.  
5. Review feedback and practice to improve.


## Contributing
Contributions are welcome! To contribute:  
1. Fork the repository.  
2. Create a feature branch (`git checkout -b feature-name`).  
3. Commit your changes (`git commit -m "Add feature"`).  
4. Push to the branch (`git push origin feature-name`).  
5. Open a Pull Request.


## Funding
- **Initial Ask:** $200K for prototype development, marketing, and hosting.  
- **ROI:** Projected 650% return over 3 years ($1.3M return on $200K investment).  

Interested in investing? Contact us at [teamaramm@gmail.com](teamaramm@gmail.com).


## Acknowledgments

- **[Eng. Tariq Elouzeh](https://tariqlabs.com)** For organizing and supporting the [Salam Hack Hackathon](https://salamhack.com) competition, providing us with this platform to compete in the Arabic world.


## License
This project is licensed under the MIT License.
