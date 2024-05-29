using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Common.Infrastructure.DataTransferObjects
{
    //Global api response type
    [Serializable]
    public record ResponseDto
    {
        public object? Data { get; set; } //use with convert class (xxx)responsedto.data;
        [Required]
        public required string Message { get; set; } //Generally error message or OK
        [Required]
        public required int Code { get; set; } //Http Status Code

        public static ResponseDto Success(object? model, bool result, string message = "")
        {
            return new ResponseDto
            {
                Data = model,
                Code = Convert.ToInt32(result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError),
                Message = result ? "OK" : message
            };
        }
    }
}