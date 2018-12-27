using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tasking
{
    public class OneNoteService
    {
        private const string ApiRoot = "https://graph.microsoft.com/v1.0/me/onenote";
        private readonly GraphApiService _graphApiService;

        public OneNoteService(GraphApiService graphApiService)
        {
            _graphApiService = graphApiService;
        }

        public async Task<IReadOnlyCollection<Notebook>> GetNotebooks()
        {
            var response = await _graphApiService.GetWithToken($"{ApiRoot}/notebooks");
            return Deserialize<Context<Notebook>>(response).Value.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Section>> GetSections(Notebook notebook)
        {
            var response = await _graphApiService.GetWithToken($"{ApiRoot}/notebooks/{notebook.Id}/sections");
            return Deserialize<Context<Section>>(response).Value.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Page>> GetPages(Section section)
        {
            var response = await _graphApiService.GetWithToken($"{ApiRoot}/sections/{section.Id}/pages");
            return Deserialize<Context<Page>>(response).Value.ToList().AsReadOnly();
        }

        public async Task<string> GetPageContent(Page page)
        {
            var response = await _graphApiService.GetWithToken($"{ApiRoot}/pages/{page.Id}/content?includeIDs=true");
            return response;
        }

        public async Task<string> AppendTaskToPage(Page page, string task)
        {
            var change = new Change
            {
                target = "body",
                action = "append",
                content = $"<p data-tag=\"to-do\" style=\"margin-top:0pt; margin-bottom:0pt\">{task}</p>"
            };
            var response = await _graphApiService.PatchWithToken(page.ContentUrl, Serialize(new[] { change }));
            return response;
        }

        public async Task<string> RemoveTaskFromPage(Page page, string id)
        {
            var change = new Change
            {
                target = id,
                action = "replace",
                content = "<!-- removed completed task --/>"
            };
            var response = await _graphApiService.PatchWithToken(page.ContentUrl, Serialize(new[] { change }));
            return response;
        }

        public async Task<string> AppendContentToPageWithTimestamp(Page page, string text)
        {
            var content = GenerateTimestampHeader() + text;
            var change = new Change
            {
                target = "body",
                action = "append",
                content = content
            };
            var response = await _graphApiService.PatchWithToken(page.ContentUrl, Serialize(new[] { change }));
            return response;
        }

        public async Task<string> AppendNoteToPageWithTimestamp(Page page, string text)
        {
            var content = $"<p style=\"margin-top:0pt;margin-bottom:0pt\"><span style=\"font-family:Consolas;font-size:10.5pt;color:black\">{text}</span></p><br/>";
            return await AppendContentToPageWithTimestamp(page, content);
        }

        private string GenerateTimestampHeader()
        {
            var date = DateTime.Now.Date.ToLongDateString();
            return
                $"<p style=\"margin-top:4pt; margin-bottom:0pt\"><span style=\"font-family:Consolas; font-size:10.5pt; color: black; font-weight:bold\"># {date}</span></p>";
        }

        private T Deserialize<T>(string content)
        {
            var result = (T)JsonConvert.DeserializeObject(content, typeof(T));
            return result;
        }

        private string Serialize<T>(T obj)
        {
            var result = JsonConvert.SerializeObject(obj);
            return result;
        }
    }
}
