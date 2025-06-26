using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PROG6221_st10440651_Part3
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private List<ActivityLog> activityLogs = new List<ActivityLog>();
        private int currentQuizQuestionIndex = -1;
        private int quizScore = 0;
        private string userName = "";
        private Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>
        {
            { "password", new List<string> { "Use strong, unique passwords.", "Avoid personal info in passwords." } },
            { "phishing", new List<string> { "Don't click suspicious links.", "Report phishing emails." } },
            { "privacy", new List<string> { "Check your account privacy settings.", "Use two-factor authentication." } }
        };
        private Dictionary<string, string> sentiments = new Dictionary<string, string>
        {
            { "worried", "It's okay to feel worried. Let me help with some tips!" },
            { "curious", "Great to see your curiosity! Here's some info." }
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuiz();
            PlayVoiceGreeting();
            DisplayAsciiArt();
            AddToLog("Application started.");
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("welcome.wav"); // Place welcome.wav in project folder
                player.Play();
                ChatHistory.Items.Add("Welcome to the Cybersecurity Awareness Chatbot!");
            }
            catch
            {
                ChatHistory.Items.Add("Voice greeting unavailable. Welcome to the Cybersecurity Awareness Chatbot!");
            }
        }

        private void DisplayAsciiArt()
        {
            AsciiArtBlock.Text = @"
   _____ _          
  / ____| |         
 | |    | |__   ___ 
 | |    | '_ \ / __|
 | |____| | | | (__ 
  \_____|_| |_|___|
";
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(input)) return;

            ChatHistory.Items.Add($"You: {input}");
            ProcessInput(input);
            UserInput.Text = "";
        }

        private void ProcessInput(string input)
        {
            // Memory: Store name
            if (input.Contains("my name is"))
            {
                userName = input.Replace("my name is", "").Trim();
                ChatHistory.Items.Add($"Bot: Nice to meet you, {userName}!");
                AddToLog($"User set name to {userName}.");
                return;
            }

            // Sentiment detection
            foreach (var sentiment in sentiments)
            {
                if (input.Contains(sentiment.Key))
                {
                    ChatHistory.Items.Add($"Bot: {sentiment.Value}");
                    AddToLog($"Detected sentiment: {sentiment.Key}");
                }
            }

            // NLP: Keyword detection
            if (input.Contains("add task") || input.Contains("set task"))
            {
                string task = input.Replace("add task", "").Replace("set task", "").Trim();
                ChatHistory.Items.Add($"Bot: Please enter task details in the Tasks tab for '{task}'.");
                AddToLog($"User requested to add task: {task}");
                return;
            }
            else if (input.Contains("quiz") || input.Contains("start quiz"))
            {
                StartQuiz();
                AddToLog("Quiz started.");
                return;
            }
            else if (input.Contains("show activity log") || input.Contains("what have you done"))
            {
                DisplayActivityLog();
                AddToLog("User viewed activity log.");
                return;
            }

            // Keyword responses
            foreach (var keyword in keywordResponses)
            {
                if (input.Contains(keyword.Key))
                {
                    string response = keyword.Value[new Random().Next(keyword.Value.Count)];
                    ChatHistory.Items.Add($"Bot: {response}");
                    AddToLog($"Responded to keyword: {keyword.Key}");
                    return;
                }
            }

            // Default response
            ChatHistory.Items.Add("Bot: I didn't understand that. Try asking about passwords, phishing, or tasks!");
            AddToLog("Unrecognized input received.");
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitle.Text.Trim();
            string description = TaskDescription.Text.Trim();
            string reminder = TaskReminder.Text.Trim();

            if (!string.IsNullOrEmpty(title))
            {
                tasks.Add(new TaskItem { Title = title, Description = description, Reminder = reminder });
                TaskList.Items.Add(new { Title = title, Description = description, Reminder = reminder });
                ChatHistory.Items.Add($"Bot: Task '{title}' added.");
                AddToLog($"Task added: {title}");
                TaskTitle.Text = TaskDescription.Text = TaskReminder.Text = "";
            }
        }

        private void MarkTaskCompleted_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem != null)
            {
                var selectedTask = tasks[TaskList.SelectedIndex];
                tasks.RemoveAt(TaskList.SelectedIndex);
                TaskList.Items.RemoveAt(TaskList.SelectedIndex);
                ChatHistory.Items.Add($"Bot: Task '{selectedTask.Title}' marked as completed.");
                AddToLog($"Task completed: {selectedTask.Title}");
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem != null)
            {
                var selectedTask = tasks[TaskList.SelectedIndex];
                tasks.RemoveAt(TaskList.SelectedIndex);
                TaskList.Items.RemoveAt(TaskList.SelectedIndex);
                ChatHistory.Items.Add($"Bot: Task '{selectedTask.Title}' deleted.");
                AddToLog($"Task deleted: {selectedTask.Title}");
            }
        }

        private void InitializeQuiz()
        {
            quizQuestions.AddRange(new[]
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectAnswer = 2,
                    Explanation = "Reporting phishing emails helps prevent scams."
                },
                new QuizQuestion
                {
                    Question = "Is it safe to use the same password for multiple accounts? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Using the same password increases risk if one account is compromised."
                },
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new[] { "Personalized greeting", "Urgent language or threats", "Correct spelling", "Official company logo" },
                    CorrectAnswer = 1,
                    Explanation = "Phishing emails often use urgent language to trick users into acting quickly."
                },
                new QuizQuestion
                {
                    Question = "Should you download attachments from unknown emails? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Downloading attachments from unknown sources can install malware."
                },
                new QuizQuestion
                {
                    Question = "What does two-factor authentication add to security?",
                    Options = new[] { "Faster login", "Extra password", "Second verification step", "Public profile" },
                    CorrectAnswer = 2,
                    Explanation = "Two-factor authentication requires a second step, like a code, to verify identity."
                },
                new QuizQuestion
                {
                    Question = "Is public Wi-Fi always safe for online banking? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Public Wi-Fi can be insecure, making it risky for sensitive tasks like banking."
                },
                new QuizQuestion
                {
                    Question = "What should a strong password include?",
                    Options = new[] { "Your name", "Numbers and symbols", "Only letters", "Your birthdate" },
                    CorrectAnswer = 1,
                    Explanation = "Strong passwords include a mix of letters, numbers, and symbols for security."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new[] { "Building social media", "Manipulating people to gain info", "Writing code", "Creating networks" },
                    CorrectAnswer = 1,
                    Explanation = "Social engineering tricks people into revealing sensitive information."
                },
                new QuizQuestion
                {
                    Question = "Should you click links in unsolicited messages? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Unsolicited links may lead to malicious sites or phishing scams."
                },
                new QuizQuestion
                {
                    Question = "What is the purpose of a firewall?",
                    Options = new[] { "Speed up internet", "Block unauthorized access", "Store passwords", "Send emails" },
                    CorrectAnswer = 1,
                    Explanation = "A firewall blocks unauthorized access to protect your network."
                }
            });
        }

        private void StartQuiz()
        {
            currentQuizQuestionIndex = 0;
            quizScore = 0;
            DisplayQuizQuestion();
        }

        private void DisplayQuizQuestion()
        {
            if (currentQuizQuestionIndex >= quizQuestions.Count)
            {
                string feedback = quizScore >= 8 ? "Great job! You're a cybersecurity pro!" : "Keep learning to stay safe online!";
                ChatHistory.Items.Add($"Bot: Quiz completed! Score: {quizScore}/{quizQuestions.Count}. {feedback}");
                AddToLog($"Quiz completed with score {quizScore}/{quizQuestions.Count}");
                currentQuizQuestionIndex = -1;
                QuizQuestion.Text = "";
                OptionA.Content = OptionB.Content = OptionC.Content = OptionD.Content = "";
                OptionA.IsChecked = OptionB.IsChecked = OptionC.IsChecked = OptionD.IsChecked = false;
                return;
            }

            var question = quizQuestions[currentQuizQuestionIndex];
            QuizQuestion.Text = question.Question;
            OptionA.Content = question.Options.Length > 0 ? question.Options[0] : "";
            OptionB.Content = question.Options.Length > 1 ? question.Options[1] : "";
            OptionC.Content = question.Options.Length > 2 ? question.Options[2] : "";
            OptionD.Content = question.Options.Length > 3 ? question.Options[3] : "";
            OptionA.IsChecked = OptionB.IsChecked = OptionC.IsChecked = OptionD.IsChecked = false;
        }

        private void SubmitQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuizQuestionIndex < 0) return;

            var question = quizQuestions[currentQuizQuestionIndex];
            int selectedAnswer = -1;
            if (OptionA.IsChecked == true) selectedAnswer = 0;
            else if (OptionB.IsChecked == true) selectedAnswer = 1;
            else if (OptionC.IsChecked == true) selectedAnswer = 2;
            else if (OptionD.IsChecked == true) selectedAnswer = 3;

            if (selectedAnswer == question.CorrectAnswer)
            {
                quizScore++;
                QuizFeedback.Text = "Correct! " + question.Explanation;
            }
            else
            {
                QuizFeedback.Text = "Incorrect. " + question.Explanation;
            }

            currentQuizQuestionIndex++;
            DisplayQuizQuestion();
        }

        private void AddToLog(string action)
        {
            activityLogs.Add(new ActivityLog { Action = action, Timestamp = DateTime.Now });
            if (activityLogs.Count > 10)
                activityLogs.RemoveAt(0);
        }

        private void DisplayActivityLog()
        {
            ActivityLogList.Items.Clear();
            foreach (var log in activityLogs.Take(5))
            {
                ActivityLogList.Items.Add($"{log.Timestamp}: {log.Action}");
            }
        }

        private void ShowMoreLogs_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogList.Items.Clear();
            foreach (var log in activityLogs)
            {
                ActivityLogList.Items.Add($"{log.Timestamp}: {log.Action}");
            }
        }
    }

    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }

    public class ActivityLog
    {
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}