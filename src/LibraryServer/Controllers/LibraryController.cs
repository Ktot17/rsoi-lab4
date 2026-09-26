using LibraryBL.OutputPorts;
using LibraryServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers;

[ApiController]
[Route("api/v1/libraries/")]
public class LibraryController(ILibraryManager libraryManager) : ControllerBase
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 10;
    private const bool DefaultShowAll = false;
    
    [HttpGet]
    public async Task<ActionResult<LibraryPaginationResponse>> GetLibraryPagination(string city,
        int page = DefaultPage, int size = DefaultSize)
    {
        var (totalAmount, libraries) = await libraryManager.GetLibraries(city, page, size);
        
        var libraryResponses = libraries.Select(l => new LibraryResponse(l)).ToList();
        
        return Ok(new LibraryPaginationResponse(page, size, totalAmount, libraryResponses));
    }

    [HttpGet("{libraryUid}/books")]
    public async Task<ActionResult<LibraryBookPaginationResponse>> GetLibraryBookPagination(string libraryUid,
        int page = DefaultPage, int size = DefaultSize, bool showAll = DefaultShowAll)
    {
        var (totalAmount, libraryBooks) = await libraryManager.GetBooks(Guid.Parse(libraryUid), showAll, page, size);
        
        var libraryBookResponses = libraryBooks.Select(l => new LibraryBookResponse(l)).ToList();
        
        return Ok(new LibraryBookPaginationResponse(page, size, totalAmount, libraryBookResponses));
    }

    [HttpPost("books")]
    public async Task<ActionResult<LibrariesAndBooksResponse>> GetLibrariesAndBooks(
        [FromBody] LibrariesAndBooksRequest request)
    {
        var (libraries, books) = 
            await libraryManager.GetLibrariesAndBooks(request.LibraryUids, request.BookUids);
        return Ok(new LibrariesAndBooksResponse(libraries, books));
    }

    [HttpPatch("{libraryUid}/books/{bookUid}")]
    public async Task<ActionResult> ChangeAvailableCount(string libraryUid, string bookUid, int count)
    {
        await libraryManager.ChangeAvailableCount(Guid.Parse(libraryUid), Guid.Parse(bookUid), count);
        return Ok();
    }
}