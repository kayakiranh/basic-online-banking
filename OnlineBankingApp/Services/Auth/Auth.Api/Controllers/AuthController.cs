using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto model, CancellationToken cancellationToken)
        {
            var response = Policy.HandleResult<LoginResponseDto>(x => !x.Result)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, context) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _authService.LoginAsync(model, cancellationToken));

            return response.Result;
        }

        [HttpPost]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto model, CancellationToken cancellationToken)
        {
            var response = Policy.HandleResult<RegisterResponseDto>(x => !x.Result)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, context) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _authService.RegisterAsync(model, cancellationToken));

            return response.Result;
        }
    }
}