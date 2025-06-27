Cybersecure Chatbot - PROG6221 Part 3

Overview

This project is a WPF application developed for PROG6221 Part 3 (student ID: st10440651). It implements a cybersecurity awareness chatbot with task management, a quiz feature, and an activity log. The application provides users with cybersecurity tips, allows them to manage tasks with reminders, and tests their knowledge through randomized quizzes.

Features





Chatbot Interface: Users can interact with a chatbot to receive detailed cybersecurity advice on topics like phishing, passwords, VPNs, and more. Supports sentiment detection and topic memory for personalized responses.



Task Management: Add, mark as completed, or delete tasks with titles, descriptions, and reminders using a DatePicker and ComboBox for time selection.



Quiz Feature: Randomized selection of 10 cybersecurity questions per session, with feedback and scoring.



Activity Log: Tracks user actions (e.g., task additions, quiz completions) with a "Show More" option.



Audio Greeting: Plays a WAV file greeting on startup (if available).



Enhanced UI: Custom styling with larger input fields, clear labels ("Title:", "Description:", "Reminder:"), and a modern color scheme.

Setup Instructions





Prerequisites:





Visual Studio 2022 or later with .NET Framework 4.8.



Ensure System.Windows.Forms and System.Drawing.Common NuGet packages are installed for system tray functionality.



A WAV file named Greetings.wav in the Resources folder (optional for audio greeting).



Installation:





Clone the repository: git clone <repository-url>



Open the solution (PROG6221_st10440651_Part3.sln) in Visual Studio.



Restore NuGet packages: Right-click the solution in Solution Explorer and select "Restore NuGet Packages".



Verify the Greetings.wav file is included in the project (Properties: Copy to Output Directory = "Copy if newer").



Running the Application:





Set PROG6221_st10440651_Part3 as the startup project.



Build and run the solution (F5 in Visual Studio).



The application launches with a greeting message, ASCII art, and an interactive chatbot interface.

Project Structure





MainWindow.xaml: Defines the UI with a grid layout, chat area (ListBox), task management (ListView, DatePicker, ComboBox), quiz tab, and activity log.



MainWindow.xaml.cs: Handles chatbot logic, task operations, quiz functionality, and activity logging.



TaskItem.cs: Defines the TaskItem class for task data (Title, Description, Reminder).



QuizQuestion.cs: Defines the QuizQuestion class for quiz questions (Question, Options, CorrectAnswer, Explanation).



ActivityLog.cs: Defines the ActivityLog class for logging user actions (Action, Timestamp).

Usage





Chatbot: Type cybersecurity-related keywords (e.g., "phishing?", "2fa") or phrases (e.g., "I'm interested in malware") in the input TextBox. Press Enter or click "Send" to receive detailed responses. The chatbot remembers your name and favorite topic.



Tasks: In the "Tasks" tab, enter a title, description, and select a reminder date/time. Click "Add Task" to save, or use "Mark Completed" or "Delete Task" for existing tasks.



Quiz: In the "Quiz" tab, click "Start Quiz" to begin a 10-question session. Select answers via radio buttons and submit. Feedback appears after 3 seconds.



Activity Log: View recent actions in the "Activity Log" tab. Click "Show More" for the full log.



Exit: Click the "Exit" button to close the application.

Deployment





Local Deployment: Build and run in Visual Studio as described above.



Azure Deployment (Optional):





Deploy to Azure App Service using Visual Studio’s "Publish" feature.



Configure the Azure Web App URL in your repository settings.



Ensure the Greetings.wav file is included in the deployment package.



Repository: <Insert GitHub repository URL here>



Azure Web App URL: <Insert Azure Web App URL here>

References





Microsoft WPF Documentation



WPF TextBox



WPF Key Events



System.Media.SoundPlayer



WPF ListBox



WPF Data Binding



W3Schools WPF



OWASP Phishing



CISA Cybersecurity Advisories



Microsoft Security Blog
