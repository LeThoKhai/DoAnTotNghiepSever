using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.Models;
using Microsoft.AspNetCore.Identity;


[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(ICommentRepository commentRepository, UserManager<IdentityUser> userManager)
    {
        _commentRepository = commentRepository;
        _userManager = userManager;
    }

    // POST: api/Comments
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostComment([FromBody] Comment comment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userExists = await _userManager.FindByIdAsync(comment.UserId);
        if (userExists == null)
        {
            return BadRequest("User does not exist.");
        }
        comment.UserName = username;
        var createdComment = await _commentRepository.AddCommentAsync(comment);
        return Ok(createdComment);
     }


    // GET: api/Comments/{courseId}
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetComments(int courseId)
    {
        var comments = await _commentRepository.GetCommentsByCourseIdAsync(courseId);
        return Ok(comments);
    }

    //// PUT: api/Comments/{id}
    //[HttpPut("{id}")]
    //[Authorize]
    //public async Task<IActionResult> UpdateComment(int id, [FromBody] Comment comment)
    //{
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    var isAdmin = User.IsInRole("Admin");

    //    if (id != comment.CommentId || (comment.UserId != userId && !isAdmin))
    //    {
    //        return Unauthorized();
    //    }

    //    var success = await _commentRepository.UpdateCommentAsync(comment);
    //    if (!success)
    //    {
    //        return StatusCode(500, "Error updating comment.");
    //    }

    //    return NoContent();
    //}



    // DELETE: api/Comments/{id}
    //[HttpDelete("{id}")]
    //[Authorize]
    //public async Task<IActionResult> DeleteComment(int id)
    //{
    //    var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    var isAdmin = User.IsInRole("Admin");

    //    var success = await _commentRepository.DeleteCommentAsync(id, username, isAdmin);
    //    if (!success)
    //    {
    //        return Unauthorized();
    //    }

    //    return NoContent();
    //}
}
