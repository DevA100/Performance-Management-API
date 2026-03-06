using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models.DTOs;

[ApiController]
[Route("api/PerformanceSurvey")]
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;

    public ResponsesController(IResponseService responseService)
    {
        _responseService = responseService;
    }
    [Authorize(Roles = "User")]
    [HttpPost("saveMultipleChoiceResponse")]
    public async Task<IActionResult> SaveResponse([FromBody] SaveMultipleChoiceResponseDto dto)
    {
        try
        {
            await _responseService.SaveResponseAsync(dto);
            return Ok(new { message = "Response saved successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "User")]
    [HttpPost("saveTextResponse")]
    public async Task<IActionResult> SaveTextResponse([FromBody] SaveTextResponseDto textResponseDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _responseService.SaveTextResponseAsync(textResponseDto);
             return Ok(new { message = "Response saved successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
   [Authorize(Roles = "Admin")]
    [HttpGet("ResponseBy-departmentId/{departmentId}")]
    public async Task<IActionResult> GetResponsesByDepartmentId(int departmentId)
    {
        var responses = await _responseService.GetResponsesByDepartmentIdAsync(departmentId);
        return Ok(responses);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("clearResponse-by-departmentId/{departmentId}")]
    public async Task<IActionResult> ClearResponsesByDepartment(int departmentId)
    {
        await _responseService.ClearResponsesByDepartmentIdAsync(departmentId);
        return Ok("Responses cleared for department " + departmentId);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("clear-all Responses")]
    public async Task<IActionResult> ClearAllResponses()
    {
        await _responseService.ClearAllResponsesAsync();
        return Ok("All responses cleared.");
    }

    [HttpGet("downloadResults")]
    public async Task<IActionResult> DownloadResponses([FromQuery] int? departmentId = null)
    {
        var fileBytes = await _responseService.ExportResponsesToExcelAsync(departmentId);

        string fileName = departmentId.HasValue
            ? $"Responses_Department_{departmentId.Value}.xlsx"
            : "All_Responses.xlsx";

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

}
