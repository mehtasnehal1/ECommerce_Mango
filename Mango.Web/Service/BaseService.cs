using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;

        }
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                string? outgoingJson = null;
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                if (requestDto.ContentType == ContentType.MultiPartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                { 
                    message.Headers.Add("Accept", "application/json");
                }

                if (withBearer == true)
                {
                    var token = _tokenProvider.GetToken();
                    if (!string.IsNullOrEmpty(token))
                        message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);

                if (requestDto.ContentType == ContentType.MultiPartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach (var property in requestDto.Data.GetType().GetProperties())
                    {
                        var value = property.GetValue(requestDto.Data);
                        if (value is FormFile)
                        {
                            var file = (FormFile)value;
                            if (file != null && file.Length > 0)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), property.Name, file.FileName);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()), property.Name);
                        }
                       
                    }
                    message.Content = content;
                }
                else
                {
                    if (requestDto.Data != null && requestDto.Data != null)
                    {
                        outgoingJson = JsonConvert.SerializeObject(requestDto.Data);
                        message.Content = new StringContent(outgoingJson, Encoding.UTF8, "application/json");
                    }
                }
                    

                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;  
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage? apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                // If not successful, return a ResponseDto containing diagnostics (status + body + outgoing JSON)
                if (!apiResponse.IsSuccessStatusCode)
                {
                    var diag = $"Status: {(int)apiResponse.StatusCode} {apiResponse.ReasonPhrase}. ResponseBody: {apiContent}";
                    if (!string.IsNullOrEmpty(outgoingJson))
                        diag += $". RequestBody: {outgoingJson}";

                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = diag
                    };
                }

                // Success: try to deserialize the API's ResponseDto payload
                var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Message = ex.Message,
                    IsSuccess = false,
                };
            }
        }
    
    }
}

