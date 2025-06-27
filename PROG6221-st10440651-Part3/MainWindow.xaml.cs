
//st10440651
// References:
// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/overview
// https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.textbox
// https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.keyeventargs
// https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer
// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/listbox
// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-bind-to-a-collection-and-display-information-based-on-selection
// https://www.w3schools.com/wpf/wpf_controls.asp
// https://owasp.org/www-community/attacks/Phishing
// https://www.cisa.gov/news-events/cybersecurity-advisories
// https://www.microsoft.com/en-us/security/blog/using 
// ASCII Logo                   https://patorjk.com/software/taag/#p=testall&f=Alpha&t=CyberSecure
// Sample Data, FAQ             https://grok.com/chat/4b63ae80-4b06-4b6b-ba38-f423368560d2
// Playing Audio in WAV format  https://stackoverflow.com/questions/71707808/how-to-add-a-wav-file-to-windows-form-application-in-visual-studio
// Delayed response             https://ironpdf.com/blog/net-help/csharp-wait-for-seconds/
// Cyber Security Terminology   https://www.metacompliance.com/cyber-security-terminology
// Sentiment Detection          https://grok.com/share/c2hhcmQtMg%3D%3D_7c76dace-d2a1-4440-816a-8434724e882c
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

namespace PROG6221_st10440651_Part3
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private List<QuizQuestion> currentQuizQuestions = new List<QuizQuestion>();
        private List<ActivityLog> activityLogs = new List<ActivityLog>();
        private int currentQuizQuestionIndex = -1;
        private int quizScore = 0;
        private string userName = "";
        private string currentTopic = ""; // Track current topic for follow-up questions
        private readonly Random random = new Random();
        private readonly Dictionary<string, string> userMemory = new Dictionary<string, string>(); // Store user preferences
        private readonly Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>
        {
            // Cybersecurity topics
            { "password", new List<string>
                {
                    "A strong password should be at least 12 characters long, include a mix of uppercase and lowercase letters, numbers, and special characters, and avoid common words or personal information.",
                    "Use unique passwords for each account to prevent credential stuffing attacks.",
                    "Avoid using easily guessable information like birthdays or pet names in passwords."
                }
            },
            { "phishing", new List<string>
                {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations.",
                    "Check email sender addresses carefully for slight misspellings or unusual domains.",
                    "Hover over links (without clicking) to verify their destination before proceeding."
                }
            },
            { "firewall", new List<string>
                {
                    "A firewall monitors and controls incoming and outgoing network traffic based on predefined security rules, acting as a barrier to protect networks from unauthorized access and threats.",
                    "Ensure your firewall is enabled to protect against unauthorized network access.",
                    "Regularly update firewall rules to address new vulnerabilities and threats."
                }
            },
            { "encryption", new List<string>
                {
                    "Encryption converts data into a secure format that can only be read with the correct key, protecting sensitive information during transmission or storage.",
                    "Use end-to-end encryption for sensitive communications like messaging or email.",
                    "Ensure websites use HTTPS to encrypt data between your browser and the server."
                }
            },
            { "malware", new List<string>
                {
                    "Malware is malicious software designed to harm devices or networks. Protect against it by using antivirus software, avoiding suspicious downloads, and keeping systems updated.",
                    "Common malware types include viruses, worms, and trojans. Regular scans can detect them.",
                    "Avoid downloading attachments from unknown sources to reduce malware risks."
                }
            },
            { "vpn", new List<string>
                {
                    "A Virtual Private Network (VPN) encrypts your internet connection, masking your IP address and protecting your data from eavesdropping on public or unsecured networks.",
                    "Choose a reputable VPN provider to ensure strong encryption and no logging.",
                    "Use a VPN when accessing sensitive accounts on public Wi-Fi for added security."
                }
            },
            { "two-factor", new List<string>
                {
                    "Two-factor authentication (2FA) adds an extra layer of security by requiring two forms of identification, like a password and a code sent to your phone, to access an account.",
                    "Enable 2FA on all critical accounts like email and banking for enhanced protection.",
                    "Use authenticator apps for 2FA instead of SMS when possible for better security."
                }
            },
            { "multi-factor", new List<string>
                {
                    "Multi-factor authentication (MFA or 2FA) enhances security by requiring multiple forms of verification, such as a password and a code from an authenticator app or biometric data.",
                    "Enable MFA/2FA on critical accounts like email, banking, and social media to significantly reduce the risk of unauthorized access.",
                    "Use authenticator apps or hardware tokens for MFA/2FA instead of SMS, as they offer stronger protection against SIM-swapping attacks."
                }
            },
            { "ransomware", new List<string>
                {
                    "Ransomware encrypts a victim's data, locking access until a ransom is paid. Regular backups and avoiding suspicious links can help prevent it.",
                    "Never pay a ransom, as it doesn't guarantee data recovery and funds cybercriminals.",
                    "Keep software updated and use antivirus tools to reduce ransomware risks."
                }
            },
            { "social engineering", new List<string>
                {
                    "Social engineering manipulates people into revealing sensitive information or performing actions, like clicking malicious links, often through impersonation or deception.",
                    "Be skeptical of unsolicited calls or emails claiming to be from trusted entities.",
                    "Verify requests for sensitive information directly with the organization."
                }
            },
            { "patching", new List<string>
                {
                    "Patching fixes security vulnerabilities in software, preventing attackers from exploiting known weaknesses to gain unauthorized access.",
                    "Enable automatic updates to ensure timely patching of software vulnerabilities.",
                    "Regularly check for patches for all devices, including IoT and mobile devices."
                }
            },
            { "ddos", new List<string>
                {
                    "A Distributed Denial-of-Service (DDoS) attack overwhelms a server with traffic to disrupt service. Mitigation includes traffic filtering and scalable infrastructure.",
                    "DDoS attacks can target websites or networks, causing downtime or slowdowns.",
                    "Use cloud-based DDoS protection services for robust defense against attacks."
                }
            },
            { "antivirus", new List<string>
                {
                    "Antivirus software detects, quarantines, and removes malicious programs like viruses, worms, and trojans, safeguarding your system from harm.",
                    "Run regular antivirus scans to catch threats early and keep definitions updated.",
                    "Choose antivirus software with real-time protection for continuous monitoring."
                }
            },
            { "data breach", new List<string>
                {
                    "A data breach occurs when unauthorized parties access sensitive information, like personal or financial data, often leading to identity theft or fraud.",
                    "Monitor accounts for unusual activity to detect potential data breaches early.",
                    "Use strong passwords and MFA to reduce the risk of data breaches."
                }
            },
            { "secure browsing", new List<string>
                {
                    "Use HTTPS websites, avoid public Wi-Fi without a VPN, disable tracking, and keep your browser updated to reduce security risks.",
                    "Clear browser cookies regularly to limit tracking and protect your privacy.",
                    "Use private browsing modes to prevent storing sensitive session data."
                }
            },
            { "backup", new List<string>
                {
                    "Regular backups ensure you can recover data after ransomware, hardware failure, or other incidents, minimizing data loss and downtime.",
                    "Store backups offline or in a separate secure location to protect against ransomware.",
                    "Test your backups periodically to ensure they can be restored successfully."
                }
            },
            { "zero-day", new List<string>
                {
                    "A zero-day exploit targets a software vulnerability unknown to the vendor, allowing attacks before a patch is available.",
                    "Use intrusion detection systems to monitor for zero-day exploit attempts.",
                    "Keep software updated to minimize the window for zero-day vulnerabilities."
                }
            },
            { "biometrics", new List<string>
                {
                    "Biometrics, like fingerprints or facial recognition, provide unique identifiers for authentication, making it harder for unauthorized users to gain access.",
                    "Combine biometrics with passwords for stronger multi-factor authentication.",
                    "Ensure biometric data is stored securely to prevent misuse if compromised."
                }
            },
            { "cloud security", new List<string>
                {
                    "Cloud security involves protecting data, applications, and infrastructure in cloud environments through encryption, access controls, and regular audits.",
                    "Use strong access controls and monitor cloud activity for suspicious behavior.",
                    "Choose cloud providers with robust security certifications like ISO 27001."
                }
            },
            { "iot", new List<string>
                {
                    "Internet of Things (IoT) devices can be vulnerable to hacking if not properly secured, risking data leaks or network compromise. Use strong passwords and updates.",
                    "Change default passwords on IoT devices to prevent easy unauthorized access.",
                    "Isolate IoT devices on a separate network to limit risks to your main network."
                }
            },
            { "penetration testing", new List<string>
                {
                    "Penetration testing simulates cyberattacks to identify vulnerabilities in systems, helping organizations strengthen their defenses before real attacks occur.",
                    "Conduct regular penetration tests to stay ahead of evolving cyber threats.",
                    "Use findings from penetration tests to prioritize security improvements."
                }
            },
            { "scam", new List<string>
                {
                    "Be cautious of unsolicited emails or messages asking for personal information. Verify the sender's legitimacy.",
                    "Scammers often create urgency. Take time to verify before acting on suspicious requests.",
                    "Avoid clicking links in unexpected messages, as they might lead to phishing sites."
                }
            },
            { "privacy", new List<string>
                {
                    "Review privacy settings on social media to control who sees your information.",
                    "Use a VPN on public Wi-Fi to protect your data from eavesdropping.",
                    "Limit sharing personal details online to maintain your privacy."
                }
            },
            { "secure wifi", new List<string>
                {
                    "Secure your Wi-Fi with a strong password and WPA3 encryption to prevent unauthorized access.",
                    "Hide your Wi-Fi network's SSID to reduce visibility to potential attackers.",
                    "Regularly monitor connected devices to detect unauthorized Wi-Fi access."
                }
            },
            { "identity theft", new List<string>
                {
                    "Identity theft occurs when someone steals your personal information to commit fraud. Monitor accounts and use strong passwords to protect yourself.",
                    "Freeze your credit to prevent unauthorized accounts being opened in your name.",
                    "Be cautious about sharing personal details online to reduce identity theft risks."
                }
            },
            { "patch management", new List<string>
                {
                    "Effective patch management involves regularly updating software to fix vulnerabilities and improve security.",
                    "Prioritize critical patches to address high-risk vulnerabilities promptly.",
                    "Automate patch deployment where possible to ensure consistent updates."
                }
            },
            { "social media security", new List<string>
                {
                    "Secure social media accounts with strong, unique passwords and enable multi-factor authentication.",
                    "Avoid sharing sensitive personal information on social media to prevent social engineering attacks.",
                    "Regularly review connected apps and revoke access to unused or suspicious ones."
                }
            },
            { "password manager", new List<string>
                {
                    "A password manager securely stores and generates complex passwords, reducing the need to remember multiple credentials.",
                    "Use a trusted password manager to create unique passwords for each account.",
                    "Ensure your password manager uses strong encryption and has a master password with MFA."
                }
            },
            { "safe downloads", new List<string>
                {
                    "Only download files from trusted sources to avoid malware infections.",
                    "Verify the integrity of downloads using checksums when available.",
                    "Scan downloaded files with antivirus software before opening them."
                }
            },
            { "incident response", new List<string>
                {
                    "An effective incident response plan involves identifying, containing, and mitigating cybersecurity incidents promptly.",
                    "Document all incidents and conduct post-incident reviews to improve security measures.",
                    "Train employees on incident response protocols to minimize damage from breaches."
                }
            },
            { "endpoint security", new List<string>
                {
                    "Endpoint security protects devices like laptops and mobiles from threats using antivirus, firewalls, and regular updates.",
                    "Ensure all endpoints have updated security software to prevent unauthorized access.",
                    "Use device encryption to safeguard data on endpoints in case of loss or theft."
                }
            },
            { "secure email", new List<string>
                {
                    "Use secure email practices like enabling MFA, avoiding suspicious attachments, and verifying sender identities.",
                    "Encrypt sensitive emails to protect their contents from interception.",
                    "Be cautious of phishing emails that mimic trusted sources to steal credentials."
                }
            },
            { "cyber hygiene", new List<string>
                {
                    "Good cyber hygiene includes using strong passwords, updating software, and avoiding suspicious links.",
                    "Regularly back up data and use antivirus software to maintain a secure digital environment.",
                    "Educate yourself on common threats like phishing to improve your cyber hygiene."
                }
            },
            // Non-cybersecurity topics for interactivity
            { "hello", new List<string>
                {
                    "Hi there! How can I assist you?",
                    "Hello! Ready to learn about cybersecurity?",
                    "Hey, great to see you! What's on your mind?"
                }
            },
            { "how are you", new List<string>
                {
                    "I'm just a bot, but I'm doing great!",
                    "Feeling as secure as a firewall! How about you?",
                    "I'm running smoothly, ready to answer your questions!"
                }
            },
            { "name", new List<string>
                {
                    "I'm Cyber Aware Chatbot!",
                    "You can call me the CyberSecure Bot!",
                    "My name's Cyber Aware, here to keep you safe!"
                }
            },
            { "purpose", new List<string>
                {
                    "My purpose is to help you understand cyber security awareness!",
                    "I'm here to provide tips and advice on staying safe online!",
                    "I exist to educate you on cybersecurity best practices!"
                }
            },
            { "ask", new List<string>
                {
                    "You can ask me about anything cyber security related!",
                    "Feel free to ask about passwords, 2FA/MFA, VPNs, or any security topic!",
                    "Got a question about staying safe online? I'm all ears... or rather, all code!"
                }
            },
            { "time", new List<string>
                {
                    $"The current time is {DateTime.Now:hh:mm tt}.",
                    $"It's {DateTime.Now:hh:mm tt} right now, perfect time to learn about cybersecurity!",
                    $"Current time: {DateTime.Now:hh:mm tt}. What's your next question?"
                }
            }
        };
        private readonly Dictionary<string, string> sentiments = new Dictionary<string, string>
        {
            { "worried", "It's understandable to feel concerned. Let's go over some practical steps to keep you safe." },
            { "curious", "That's a great mindset! Let me share some insights to fuel your curiosity." },
            { "frustrated", "I hear you, it can be overwhelming. Let's break this down step-by-step to make it easier." }
        };
        private readonly Dictionary<string, string> acronymToTopicMap = new Dictionary<string, string>
        {
            { "2fa", "multi-factor" },
            { "mfa", "multi-factor" },
            { "vpn", "vpn" },
            { "ddos", "ddos" },
            { "iot", "iot" },
            { "ids", "endpoint security" },
            { "siem", "incident response" }
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuiz();
            InitializeTimeComboBox();
            ChatHistory.Items.Add("Bot: Welcome to the Cybersecure Chatbot! How can I assist you today?");
            PlayVoiceGreeting();
            DisplayAsciiArt();
            AddToLog("Application started.");
        }

        private void InitializeTimeComboBox()
        {
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
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string audioPath = Path.Combine(basePath, "Resources", "Greetings.wav");
                AddToLog($"Attempting to play audio from: {audioPath}");

                if (!File.Exists(audioPath))
                {
                    throw new FileNotFoundException($"Audio file not found at: {audioPath}");
                }

                SoundPlayer player = new SoundPlayer(audioPath);
                player.Load();
                player.Play();
                AddToLog("Voice greeting played successfully.");
            }
            catch (Exception ex)
            {
                ChatHistory.Items.Add($"Bot: Voice greeting unavailable: {ex.Message}.");
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
            if (string.IsNullOrEmpty(input))
            {
                ChatHistory.Items.Add("Bot: Please type a message to send!");
                return;
            }

            ChatHistory.Items.Add($"You: {input}");
            ProcessInput(input);
            UserInput.Text = "";
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            AddToLog("Application exited.");
            Application.Current.Shutdown();
        }

        private void ProcessInput(string input)
        {
            // Clean input by removing question marks and trimming
            string cleanedInput = input.Replace("?", "").Trim().ToLower();

            // Memory: Store name
            if (cleanedInput.Contains("my name is"))
            {
                userName = cleanedInput.Replace("my name is", "").Trim();
                userMemory["name"] = userName;
                ChatHistory.Items.Add($"Bot: Nice to meet you, {userName}! How can I assist you with cybersecurity today?");
                AddToLog($"User set name to {userName}.");
                return;
            }

            // Sentiment detection
            string sentiment = sentiments.Keys.FirstOrDefault(s => cleanedInput.Contains(s));
            if (!string.IsNullOrEmpty(sentiment))
            {
                ChatHistory.Items.Add($"Bot: {sentiments[sentiment]}");
                AddToLog($"Detected sentiment: {sentiment}");
            }

            // Store favorite topic
            if (cleanedInput.Contains("interested in") || cleanedInput.Contains("like to learn"))
            {
                foreach (var topic in keywordResponses.Keys)
                {
                    if (cleanedInput.Contains(topic) && topic != "name") // Avoid conflict with 'identity theft'
                    {
                        userMemory["favoriteTopic"] = topic;
                        ChatHistory.Items.Add($"Bot: Great! I'll remember you're interested in {topic}. It's a key part of staying safe online.");
                        currentTopic = topic;
                        AddToLog($"User set favorite topic to {topic}");
                        return;
                    }
                }
                foreach (var acronym in acronymToTopicMap.Keys)
                {
                    if (cleanedInput.Contains(acronym))
                    {
                        string topic = acronymToTopicMap[acronym];
                        userMemory["favoriteTopic"] = topic;
                        ChatHistory.Items.Add($"Bot: Great! I'll remember you're interested in {topic}. It's a key part of staying safe online.");
                        currentTopic = topic;
                        AddToLog($"User set favorite topic to {topic}");
                        return;
                    }
                }
            }

            // Handle task-related input
            if (cleanedInput.Contains("add task") || cleanedInput.Contains("set task"))
            {
                string task = cleanedInput.Replace("add task", "").Replace("set task", "").Trim();
                ChatHistory.Items.Add($"Bot: Please enter task details in the Tasks tab for '{task}'.");
                AddToLog($"User requested to add task: {task}");
                return;
            }
            else if (cleanedInput.Contains("quiz") || cleanedInput.Contains("start quiz"))
            {
                StartQuiz();
                AddToLog("Quiz started via chat input.");
                return;
            }
            else if (cleanedInput.Contains("show activity log") || cleanedInput.Contains("what have you done"))
            {
                DisplayActivityLog();
                AddToLog("User viewed activity log.");
                return;
            }

            // Handle follow-up questions
            if ((cleanedInput.Contains("more") || cleanedInput.Contains("details")) && !string.IsNullOrEmpty(currentTopic))
            {
                if (keywordResponses.ContainsKey(currentTopic))
                {
                    string response = keywordResponses[currentTopic][random.Next(keywordResponses[currentTopic].Count)];
                    ChatHistory.Items.Add($"Bot: {response}");
                    AddToLog($"Follow-up response for topic: {currentTopic}");
                    return;
                }
            }

            // Handle acronym matches
            foreach (var acronym in acronymToTopicMap.Keys)
            {
                string acronymLower = acronym.ToLower();
                if (cleanedInput.Equals(acronymLower) || cleanedInput.Contains($" {acronymLower} ") || cleanedInput.StartsWith($"{acronymLower} ") || cleanedInput.EndsWith($" {acronymLower}"))
                {
                    string topic = acronymToTopicMap[acronym];
                    currentTopic = topic;
                    string response = keywordResponses[topic][random.Next(keywordResponses[topic].Count)];
                    ChatHistory.Items.Add($"Bot: {response}");
                    AddToLog($"Responded to acronym: {acronymLower} (mapped to {topic})");
                    return;
                }
            }

            // Handle keyword responses
            foreach (var keyword in keywordResponses.Keys)
            {
                string keywordLower = keyword.ToLower();
                if (keyword == "name" && cleanedInput.Contains("identity")) continue; // Avoid conflict with 'identity theft'
                if (cleanedInput.Equals(keywordLower) || cleanedInput.Contains($" {keywordLower} ") || cleanedInput.StartsWith($"{keywordLower} ") || cleanedInput.EndsWith($" {keywordLower}"))
                {
                    currentTopic = keyword;
                    string response = keywordResponses[keyword][random.Next(keywordResponses[keyword].Count)];
                    // Personalize response if favorite topic is known
                    if (userMemory.ContainsKey("favoriteTopic") && random.Next(0, 3) == 0)
                    {
                        response += $"\nAs someone interested in {userMemory["favoriteTopic"]}, you might want to explore this topic further!";
                    }
                    ChatHistory.Items.Add($"Bot: {response}");
                    AddToLog($"Responded to keyword: {keywordLower}");
                    return;
                }
            }
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
            currentQuizQuestions.Clear();
            currentQuizQuestionIndex = 0;
            quizScore = 0;
            QuizQuestion.Text = "";
            QuizFeedback.Text = "";

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

            await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Render);
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