﻿using Auth.Application.DataTransferObjects;
using Auth.Application.Interfaces;
using Common.Infrastructure.DataTransferObjects;
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
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginRequestDto model, CancellationToken cancellationToken)
        {
            Task<LoginResponseDto> response = Policy.HandleResult<LoginResponseDto>(x => !x.Result)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _authService.LoginAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Result, "İşlem yapılırken bir hata meydana geldi");
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] RegisterRequestDto model, CancellationToken cancellationToken)
        {
            Task<RegisterResponseDto> response = Policy.HandleResult<RegisterResponseDto>(x => !x.Result)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(30), (_, timeSpan, retryCount, _) => Console.WriteLine($"Retry : {retryCount}, Waiting : {timeSpan} "))
            .ExecuteAsync(async () => await _authService.RegisterAsync(model, cancellationToken));

            return ResponseDto.Success(response, response.Result.Result, "İşlem yapılırken bir hata meydana geldi");
        }
    }
}