using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;

namespace API.Controllers
    {
    [Authorize]
    public class UserController : BaseApiController
        {
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPhotoRepository photoRepository;
        private readonly IMapper mapper;
        

        public UserController( UserManager<AppUser> userManager,IUnitOfWork unitOfWork,IPhotoRepository photoRepository  ,IMapper mapper )
            {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.photoRepository = photoRepository;
            this.mapper = mapper;
            }
        //[AllowAnonymous]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AppUser>> GetUser( int id )
        //    {
        //    return await userRepository.GetUserByIdAsync(id);
        //    }
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams )
            {
            var gender = await unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();
             if(string.IsNullOrEmpty(userParams.Gender))
                {
                userParams.Gender = gender == "male" ? "female" : "male";
                }
            var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);
            }
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username )
            {
            //    var user =await userRepository.GetUserByUsernameAsync(username );
            //return  mapper.Map<MemberDto>(user);
            var currentUsername = User.GetUsername();
            return await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: currentUsername== username);
            }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto updateDto )
            {
            var user=await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null)
                return NotFound();
            mapper.Map(updateDto, user);
            if (await unitOfWork.Complate())
                return NoContent();
            return BadRequest("Failed to update user");
            }
       
        /********************************************************/
        public async Task<ActionResult<PhotoDto>> AddPhotoWithUsername(string PatientUsername, IFormFile file )
            {
            var user = await
        unitOfWork.UserRepository.GetUserByUsernameAsync(PatientUsername);

            var result = await photoRepository.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
            var photo = new Photo
                {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
                };
            user.Photos.Add(photo);
            if (await unitOfWork.Complate())
                {
                return CreatedAtRoute("GetUser", new
                    {
                    username =
               user.UserName
                    }, mapper.Map<PhotoDto>(photo));
                }
            return BadRequest("Problem addding photo");
            }
        /********************************************************/

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto( IFormFile file )
            {
            var user = await
        unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var result = await photoRepository.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
            var photo = new Photo
                {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
                };
            user.Photos.Add(photo);
            if (await unitOfWork.Complate())
                {
                return CreatedAtRoute("GetUser", new
                    {
                    username =
               user.UserName
                    }, mapper.Map<PhotoDto>(photo));
                }
            return BadRequest("Problem addding photo");
            }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId )
            {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();
          
            var photo=user.Photos.FirstOrDefault(p => p.Id==photoId);
            
            if (photo == null)
                return NotFound();
            if (photo.IsMain)
                return BadRequest("this is already your main photo");

            var currentMain=user.Photos.FirstOrDefault(p=>p.IsMain);
            if (currentMain != null)
                currentMain.IsMain = false;
            photo.IsMain = true;
            if (await unitOfWork.Complate())
                return NoContent();
            return BadRequest("Problem setting this Photo!");
            }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto( int photoId )
            {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = await unitOfWork.photoRepo.GetPhotoById(photoId);

            if (photo == null)
                return NotFound();

            if (photo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
                {
                var result = await photoRepository.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    return BadRequest(result.Error.Message);
                }

            user.Photos.Remove(photo);

            if (await unitOfWork.Complate())
                return Ok();

            return BadRequest("Problem deleting photo");
            }



        }
    }
