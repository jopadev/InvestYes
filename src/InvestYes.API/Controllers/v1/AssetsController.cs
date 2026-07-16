using Asp.Versioning;
using InvestYes.Application.DTOs;
using InvestYes.Application.Features.Assets.Commands.CreateAsset;
using InvestYes.Application.Features.Assets.Commands.DeleteAsset;
using InvestYes.Application.Features.Assets.Commands.UpdateAsset;
using InvestYes.Application.Features.Assets.Queries.CompareAssets;
using InvestYes.Application.Features.Assets.Queries.GetAssets;
using InvestYes.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestYes.API.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/assets")]
[Produces("application/json")]
public sealed class AssetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cadastra um novo ativo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAssetCommand command,CancellationToken cancellationToken)
    {
        var oAssetDto = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll),new { id = oAssetDto.AssetId }, oAssetDto);
    }

    /// <summary>
    /// Atualiza um ativo.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateAssetCommand command,CancellationToken cancellationToken)
    {
        var oAssetDto = await _mediator.Send(command, cancellationToken);

        return Ok(oAssetDto);
    }

    /// <summary>
    /// Remove um ativo.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAssetCommand(id),cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Lista todos os ativos.
    /// </summary>
    [HttpGet("todos")]
    [ProducesResponseType(typeof(IEnumerable<AssetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var oAssetDtoList = await _mediator.Send(new GetAssetsQuery(),cancellationToken);

        return Ok(oAssetDtoList);
    }

    /// <summary>
    /// Analisa um ativo.
    /// </summary>
    [HttpGet("analisar")]
    [ProducesResponseType(typeof(AssetAnalysisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Analyze([FromQuery] string ticker,CancellationToken cancellationToken)
    {
        var oAssetAnalysisDto = await _mediator.Send(new AnalyzeAssetQuery(ticker),cancellationToken);

        return Ok(oAssetAnalysisDto);
    }

    /// <summary>
    /// Compara dois ativos.
    /// </summary>
    [HttpGet("comparar")]
    [ProducesResponseType(typeof(AssetComparisonDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Compare([FromQuery] string asset1,[FromQuery] string asset2,CancellationToken cancellationToken)
    {
        var oAssetComparisonDto = await _mediator.Send(new CompareAssetsQuery(asset1, asset2),cancellationToken);

        return Ok(oAssetComparisonDto);
    }
}