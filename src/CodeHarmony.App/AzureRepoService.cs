using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace CodeHarmony.App
{
	public class AzureRepoService
	{
		private readonly Uri _uri;
		private readonly VssConnection _connection;

		public AzureRepoService(string organization, string personalAccessToken)
		{
			_uri = new Uri($"https://dev.azure.com/{organization}");
			var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
			_connection = new VssConnection(_uri, credentials);
		}

		public async Task<List<GitRepository>> GetRepositoriesAsync()
		{
			var gitClient = _connection.GetClient<GitHttpClient>();
			var repositories = await gitClient.GetRepositoriesAsync();
			return repositories;
		}
	}
}