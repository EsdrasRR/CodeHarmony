using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;

namespace CodeHarmony.App
{
    public partial class MockForm : Form
    {
		private readonly IConfiguration _configuration;

		public MockForm()
		{
            InitializeComponent();

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

			_configuration = builder.Build();
		}

        private void InitializeComponent()
        {
            this.mockButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mockButton
            // 
            this.mockButton.Location = new System.Drawing.Point(100, 100);
            this.mockButton.Name = "mockButton";
            this.mockButton.Size = new System.Drawing.Size(100, 23);
            this.mockButton.TabIndex = 0;
            this.mockButton.Text = "Mock Service";
            this.mockButton.UseVisualStyleBackColor = true;
            this.mockButton.Click += new System.EventHandler(this.MockButton_Click);
            // 
            // MockForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.mockButton);
            this.Name = "MockForm";
            this.ResumeLayout(false);
        }

        private async void MockButton_Click(object sender, EventArgs e)
        {
			string basePath = @"C:\MyRepo\CodeHarmonyRepos";

			string? organization = _configuration["Azure:Organization"];
			string? personalAccessToken = _configuration["Azure:PersonalAccessToken"];

            if (organization is not null && personalAccessToken is not null)
            {
				var service = new AzureRepoService(organization, personalAccessToken);
				var repositories = await service.GetRepositoriesAsync();

				var groupedRepositories = repositories
					.GroupBy(repo => ExtractNamespaceFromUrl(repo.RemoteUrl))
					.ToDictionary(g => g.Key, g => g.ToList());

				foreach (var group in groupedRepositories)
				{
					string groupFolderPath = Path.Combine(basePath, group.Key);
					Directory.CreateDirectory(groupFolderPath);

					foreach (var repo in group.Value)
					{
						string repoPath = Path.Combine(groupFolderPath, repo.Name);

						if (!Directory.Exists(repoPath))
						{
							CloneRepository(repo.RemoteUrl, repoPath);
						}
					}
				}

				MessageBox.Show("Repositories cloned successfully.");

			}
			else
            {
				MessageBox.Show($"Organization or Personal Access Token invalid");
			}
		}

		private void CloneRepository(string repoUrl, string repoPath)
		{
			// Ensure the repoUrl and repoPath are correctly formatted
			repoUrl = repoUrl.Trim();
			repoPath = repoPath.Trim();

			var processInfo = new ProcessStartInfo("git", $"clone \"{repoUrl}\" \"{repoPath}\"")
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false
			};

			using (var process = new Process())
			{
				process.StartInfo = processInfo;
				process.Start();

				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					MessageBox.Show($"Error cloning repository: {error}");
				}
			}
		}

		private string ExtractNamespaceFromUrl(string url)
		{
			var parts = url.Split('/');
			string namespacePart = parts.Length > 4 ? parts[4] : string.Empty;
			return WebUtility.UrlDecode(namespacePart);
		}

		private System.Windows.Forms.Button mockButton;
    }
}
