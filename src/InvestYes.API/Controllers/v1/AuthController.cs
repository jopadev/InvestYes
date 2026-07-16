using InvestYes.Application.Features.Login;
using InvestYes.Application.Features.Login.Commands;
using InvestYes.Application.Features.Register.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestYes.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var oLoginResponse = await _mediator.Send(command);

            return Ok(oLoginResponse);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var oRegisterResponse = await _mediator.Send(command);

            return CreatedAtAction(nameof(Register),new { id = oRegisterResponse.Id }, oRegisterResponse);
        }
    }
}
