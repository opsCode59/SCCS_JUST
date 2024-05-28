using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
    {
    public class LikeController : BaseApiController
        {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public LikeController(IUnitOfWork unitOfWork,UserManager<AppUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike( string username )
            {
            var sourceUserId = User.GetUserId();
            var likedUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await unitOfWork.likeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();

            if (sourceUser.UserName == username)
                return BadRequest("You cannot like yourself");

            // Check if the source user already has a liked user
            if (sourceUser.LikedUser != null)
                return BadRequest("You can only Subscribe in one Clinic");

            // Check if the liked user is already liked by someone else
            if (likedUser.LikedByUser != null)
                return BadRequest("This user is already liked by someone else");

            var userLike = new UserLike
                {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
                };

            sourceUser.LikedUser = userLike;
            likedUser.LikedByUser = userLike;

            if (await unitOfWork.Complate())
                return Ok();

            return BadRequest("Failed to like user");
            }


        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams )
            {
            likesParams.userId = User.GetUserId();
            var users = await unitOfWork.likeRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);
            }

    }
    }
