using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Tasking
{
    public partial class Form1 : Form
    {
        private readonly GraphApiService _graphApiService;
        private readonly OneNoteService _oneNoteService;
        private Page _tasksPage;
        private Page _journalPage;

        public Form1()
        {
            InitializeComponent();
            _graphApiService = new GraphApiService();
            _oneNoteService = new OneNoteService(_graphApiService);
            button1.Show();
            button2.Hide();
            button4.Enabled = false;
            textBox2.Enabled = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await _graphApiService.Login();
            var notebooks = await _oneNoteService.GetNotebooks();
            var sections = await _oneNoteService.GetSections(notebooks.First());
            var pages = await _oneNoteService.GetPages(sections.First(s => s.DisplayName == "Journal"));
            _tasksPage = pages.First(p => p.Title == "Tasks");
            _journalPage = pages.First(p => p.Title == "Journal");
            button1.Hide();
            button2.Show();
            button4.Enabled = true;
            textBox2.Enabled = true;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await _graphApiService.Logout();
            Close();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var content = await _oneNoteService.GetPageContent(_journalPage);
            textBox1.Text = content;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            button4.Enabled = false;
            var text = textBox2.Text;
            if (text.StartsWith("task"))
            {
                text = text.Substring(4).Trim();
                await _oneNoteService.AppendTaskToPage(_tasksPage, text);
            }
            else
            {
                await _oneNoteService.AppendNoteToPageWithTimestamp(_journalPage, text);
            }
            textBox2.Enabled = true;
            button4.Enabled = true;
            textBox2.Text = "";
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var tasksContent = await _oneNoteService.GetPageContent(_tasksPage);
            var dom = new System.Xml.XmlDocument();
            dom.LoadXml(tasksContent);
            var finishedTasks = dom.SelectNodes("//p[@data-tag=\"to-do:completed\"]");
            if (finishedTasks != null)
            {
                foreach (var finishedTask in finishedTasks)
                {
                    if (finishedTask is XmlNode node)
                    {
                        var id = node.Attributes["id"].Value;
                        await _oneNoteService.RemoveTaskFromPage(_tasksPage, id);
                        await _oneNoteService.AppendContentToPageWithTimestamp(_journalPage, node.OuterXml);

                    }
                }
            }
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                //Show();
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                //Hide();
            }
        }
    }
}
