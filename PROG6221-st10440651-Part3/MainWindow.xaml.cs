using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace PROG6221_st10440651_Part3
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private List<QuizQuestion> currentQuizQuestions = new List<QuizQuestion>(); // For randomized 10 questions
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
            InitializeTimeComboBox();
            PlayVoiceGreeting();
            DisplayAsciiArt();
            AddToLog("Application started.");
        }

        private void InitializeTimeComboBox()
        {
            // Populate TaskReminderTime with 30-minute intervals (00:00 to 23:30)
            for (int hour = 0; hour < 24; hour++)
            {
                TaskReminderTime.Items.Add($"{hour:D2}:00");
                TaskReminderTime.Items.Add($"{hour:D2}:30");
            }
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                // Get the absolute path to Resources/Greetings.wav
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string audioPath = Path.Combine(basePath, "Resources", "Greetings.wav");
                AddToLog($"Attempting to play audio from: {audioPath}");

                if (!File.Exists(audioPath))
                {
                    throw new FileNotFoundException($"Audio file not found at: {audioPath}");
                }

                SoundPlayer player = new SoundPlayer(audioPath);
                player.Load(); // Load the file to validate it
                player.Play();
                ChatHistory.Items.Add("Welcome to the Cybersecure Chatbot!");
                AddToLog("Voice greeting played successfully.");
            }
            catch (Exception ex)
            {
                ChatHistory.Items.Add($"Voice greeting unavailable: {ex.Message}. Welcome to the Cybersecure Chatbot!");
                AddToLog($"Failed to play voice greeting: {ex.Message}");
            }
        }

        private void DisplayAsciiArt()
        {
            AsciiArtBlock.Text = @"
 
    __  __ __  ____     ___  ____    _____   ___    __  __ __  ____     ___ 
   /  ]|  |  ||    \   /  _]|    \  / ___/  /  _]  /  ]|  |  ||    \   /  _]
  /  / |  |  ||  o  ) /  [_ |  D  )(   \_  /  [_  /  / |  |  ||  D  ) /  [_ 
 /  /  |  ~  ||     ||    _]|    /  \__  ||    _]/  /  |  |  ||    / |    _]
/   \_ |___, ||  O  ||   [_ |    \  /  \ ||   [_/   \_ |  :  ||    \ |   [_ 
\     ||     ||     ||     ||  .  \ \    ||     \     ||     ||  .  \|     |
 \____||____/ |_____||_____||__|\_|  \___||_____|\____| \__,_||__|\_||_____|
 

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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            AddToLog("Application exited.");
            Application.Current.Shutdown();
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
                AddToLog("Quiz started via chat input.");
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

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
            AddToLog("Quiz started via button.");
        }

        private void EndQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizQuestion.Text = "Quiz ended! Click 'Start Quiz' to try again.";
            OptionA.Visibility = Visibility.Collapsed;
            OptionB.Visibility = Visibility.Collapsed;
            OptionC.Visibility = Visibility.Collapsed;
            OptionD.Visibility = Visibility.Collapsed;
            SubmitAnswerButton.Visibility = Visibility.Collapsed;
            EndQuizButton.Visibility = Visibility.Collapsed;
            QuizFeedback.Text = "";
            string feedback = quizScore >= 8 ? "Great job! You're a cybersecurity pro!" : "Keep learning to stay safe online!";
            ChatHistory.Items.Add($"Bot: Quiz ended early! Score: {quizScore}/{currentQuizQuestions.Count}. {feedback}");
            AddToLog($"Quiz ended early with score {quizScore}/{currentQuizQuestions.Count}");
            currentQuizQuestionIndex = -1;
            currentQuizQuestions.Clear();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitle.Text.Trim();
            string description = TaskDescription.Text.Trim();
            string reminder = "";

            if (TaskReminderDate.SelectedDate.HasValue && TaskReminderTime.SelectedItem != null)
            {
                DateTime date = TaskReminderDate.SelectedDate.Value;
                string time = TaskReminderTime.SelectedItem.ToString();
                reminder = $"{date:yyyy-MM-dd} {time}";
            }

            if (!string.IsNullOrEmpty(title))
            {
                tasks.Add(new TaskItem { Title = title, Description = description, Reminder = reminder });
                TaskList.Items.Add(new { Title = title, Description = description, Reminder = reminder });
                ChatHistory.Items.Add($"Bot: Task '{title}' added.");
                AddToLog($"Task added: {title}");
                TaskTitle.Text = "";
                TaskDescription.Text = "";
                TaskReminderDate.SelectedDate = null;
                TaskReminderTime.SelectedItem = null;
            }
            else
            {
                ChatHistory.Items.Add("Bot: Please enter a task title.");
                AddToLog("Task addition failed: No title provided.");
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
                },
                new QuizQuestion
                {
                    Question = "What is malware?",
                    Options = new[] { "A type of hardware", "Malicious software", "A secure protocol", "An encryption method" },
                    CorrectAnswer = 1,
                    Explanation = "Malware is malicious software designed to harm or exploit devices."
                },
                new QuizQuestion
                {
                    Question = "Is it safe to share your password with a friend? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Sharing passwords increases the risk of unauthorized access."
                },
                new QuizQuestion
                {
                    Question = "What does a VPN do?",
                    Options = new[] { "Speeds up your internet", "Encrypts your connection", "Blocks ads", "Stores data" },
                    CorrectAnswer = 1,
                    Explanation = "A VPN encrypts your internet connection to protect your data."
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new[] { "Software to speed up your PC", "Malware that locks your data", "A type of firewall", "A password manager" },
                    CorrectAnswer = 1,
                    Explanation = "Ransomware locks your data and demands payment to unlock it."
                },
                new QuizQuestion
                {
                    Question = "Should you update your software regularly? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 0,
                    Explanation = "Regular updates patch security vulnerabilities."
                },
                new QuizQuestion
                {
                    Question = "What is a brute force attack?",
                    Options = new[] { "Sending phishing emails", "Trying many passwords", "Installing malware", "Hacking a firewall" },
                    CorrectAnswer = 1,
                    Explanation = "A brute force attack tries multiple passwords to gain access."
                },
                new QuizQuestion
                {
                    Question = "Is it safe to use 'password' as your password? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Common passwords like 'password' are easily guessed by attackers."
                },
                new QuizQuestion
                {
                    Question = "What does HTTPS indicate on a website?",
                    Options = new[] { "High-speed connection", "Secure connection", "Free website", "No login required" },
                    CorrectAnswer = 1,
                    Explanation = "HTTPS indicates the website uses encryption to secure data."
                },
                new QuizQuestion
                {
                    Question = "What is a keylogger?",
                    Options = new[] { "A password manager", "Software that records keystrokes", "A network scanner", "An encryption tool" },
                    CorrectAnswer = 1,
                    Explanation = "A keylogger records keystrokes to steal sensitive information."
                },
                new QuizQuestion
                {
                    Question = "Should you back up your data regularly? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 0,
                    Explanation = "Regular backups protect against data loss from attacks or failures."
                },
                new QuizQuestion
                {
                    Question = "What is encryption used for?",
                    Options = new[] { "Speeding up data", "Protecting data confidentiality", "Increasing storage", "Blocking websites" },
                    CorrectAnswer = 1,
                    Explanation = "Encryption protects data by making it unreadable without a key."
                },
                new QuizQuestion
                {
                    Question = "Can antivirus software detect all malware? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "No antivirus can detect all malware, but it significantly reduces risk."
                },
                new QuizQuestion
                {
                    Question = "What is a DDoS attack?",
                    Options = new[] { "Stealing passwords", "Overloading a server", "Encrypting data", "Phishing emails" },
                    CorrectAnswer = 1,
                    Explanation = "A DDoS attack overloads a server to disrupt service availability."
                },
                new QuizQuestion
                {
                    Question = "Is it safe to connect to unknown USB devices? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Unknown USB devices can contain malware that infects your system."
                },
                new QuizQuestion
                {
                    Question = "What is a password manager used for?",
                    Options = new[] { "Hacking accounts", "Storing secure passwords", "Blocking websites", "Sending emails" },
                    CorrectAnswer = 1,
                    Explanation = "A password manager securely stores and manages your passwords."
                },
                new QuizQuestion
                {
                    Question = "Should you use public computers for sensitive tasks? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Public computers may have keyloggers or malware, making them unsafe."
                },
                new QuizQuestion
                {
                    Question = "What is a zero-day exploit?",
                    Options = new[] { "A new software feature", "An unknown vulnerability", "A strong password", "A firewall rule" },
                    CorrectAnswer = 1,
                    Explanation = "A zero-day exploit targets a vulnerability unknown to the software vendor."
                },
                new QuizQuestion
                {
                    Question = "Does incognito mode protect your data from hackers? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Incognito mode only prevents browser history saving, not hacking."
                },
                new QuizQuestion
                {
                    Question = "What is multi-factor authentication?",
                    Options = new[] { "Multiple passwords", "Multiple verification methods", "Multiple users", "Multiple devices" },
                    CorrectAnswer = 1,
                    Explanation = "Multi-factor authentication uses multiple verification methods for security."
                },
                new QuizQuestion
                {
                    Question = "Should you trust emails claiming you won a prize? (True/False)",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Such emails are often scams to steal personal information."
                }
            });
        }

        private void StartQuiz()
        {
            // Clear previous quiz questions and reset state
            currentQuizQuestions.Clear();
            currentQuizQuestionIndex = 0;
            quizScore = 0;
            QuizQuestion.Text = "";
            QuizFeedback.Text = "";

            // Randomly select 10 questions
            Random random = new Random();
            currentQuizQuestions = quizQuestions.OrderBy(x => random.Next()).Take(10).ToList();

            OptionA.Visibility = Visibility.Visible;
            OptionB.Visibility = Visibility.Visible;
            OptionC.Visibility = Visibility.Visible;
            OptionD.Visibility = Visibility.Visible;
            SubmitAnswerButton.Visibility = Visibility.Visible;
            EndQuizButton.Visibility = Visibility.Visible;
            DisplayQuizQuestion();
        }

        private void DisplayQuizQuestion()
        {
            if (currentQuizQuestionIndex < 0 || currentQuizQuestionIndex >= currentQuizQuestions.Count)
            {
                QuizQuestion.Text = "Quiz completed! Click 'Start Quiz' to try again.";
                OptionA.Visibility = Visibility.Collapsed;
                OptionB.Visibility = Visibility.Collapsed;
                OptionC.Visibility = Visibility.Collapsed;
                OptionD.Visibility = Visibility.Collapsed;
                SubmitAnswerButton.Visibility = Visibility.Collapsed;
                EndQuizButton.Visibility = Visibility.Collapsed;
                QuizFeedback.Text = "";
                string feedback = quizScore >= 8 ? "Great job! You're a cybersecurity pro!" : "Keep learning to stay safe online!";
                ChatHistory.Items.Add($"Bot: Quiz completed! Score: {quizScore}/{currentQuizQuestions.Count}. {feedback}");
                AddToLog($"Quiz completed with score {quizScore}/{currentQuizQuestions.Count}");
                currentQuizQuestionIndex = -1;
                currentQuizQuestions.Clear();
                return;
            }

            var question = currentQuizQuestions[currentQuizQuestionIndex];
            QuizQuestion.Text = question.Question;
            OptionA.Content = question.Options.Length > 0 ? question.Options[0] : "";
            OptionB.Content = question.Options.Length > 1 ? question.Options[1] : "";
            OptionC.Content = question.Options.Length > 2 ? question.Options[2] : "";
            OptionD.Content = question.Options.Length > 3 ? question.Options[3] : "";
            OptionA.Visibility = question.Options.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            OptionB.Visibility = question.Options.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
            OptionC.Visibility = question.Options.Length > 2 ? Visibility.Visible : Visibility.Collapsed;
            OptionD.Visibility = question.Options.Length > 3 ? Visibility.Visible : Visibility.Collapsed;
            OptionA.IsChecked = OptionB.IsChecked = OptionC.IsChecked = OptionD.IsChecked = false;
            QuizFeedback.Text = "";
        }

        private async void SubmitQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuizQuestionIndex < 0 || currentQuizQuestionIndex >= currentQuizQuestions.Count) return;

            var question = currentQuizQuestions[currentQuizQuestionIndex];
            int selectedAnswer = -1;
            if (OptionA.IsChecked == true) selectedAnswer = 0;
            else if (OptionB.IsChecked == true) selectedAnswer = 1;
            else if (OptionC.IsChecked == true) selectedAnswer = 2;
            else if (OptionD.IsChecked == true) selectedAnswer = 3;

            if (selectedAnswer == -1)
            {
                QuizFeedback.Text = "Please select an answer.";
                return;
            }

            string correctAnswerText = question.Options[question.CorrectAnswer];
            string feedbackText;
            if (selectedAnswer == question.CorrectAnswer)
            {
                quizScore++;
                feedbackText = $"Correct! The correct answer is: {correctAnswerText}. {question.Explanation}";
            }
            else
            {
                feedbackText = $"Incorrect :( The correct answer is: {correctAnswerText}. {question.Explanation}";
            }

            QuizFeedback.Text = feedbackText;
            AddToLog($"Answered question {currentQuizQuestionIndex + 1}: {feedbackText}");

            // Force UI update
            await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Render);
            // Delay for 3 seconds for feedback visibility
            await Task.Delay(3000);

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
}