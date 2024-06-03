using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace TranscriptionAPIOpenAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranscriptionController : ControllerBase
    {
        [HttpPost]
        [Route("transcribe")]
        public async Task<IActionResult> TranscribeAudio(IFormFile audioFile)
        {
            try
            {
                var apiKey = "Sua_Chave";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                using var content = new MultipartFormDataContent();
                using var fileStream = audioFile.OpenReadStream();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

                content.Add(streamContent, "file", audioFile.FileName);
                content.Add(new StringContent("whisper-1"), "model");

                var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

                var transcription = await response.Content.ReadAsStringAsync();
                return Ok(transcription);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
