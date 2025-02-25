using Microsoft.Extensions.Configuration;

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
			// Mock parameters
			string? organization = _configuration["Azure:Organization"];
			string? personalAccessToken = _configuration["Azure:PersonalAccessToken"];

            if (organization is not null && personalAccessToken is not null)
            {
				var service = new AzureRepoService(organization, personalAccessToken);
				var repositories = await service.GetRepositoriesAsync();
			}
		}

        private System.Windows.Forms.Button mockButton;
    }
}
