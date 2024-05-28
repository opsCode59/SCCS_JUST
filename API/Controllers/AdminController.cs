using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
    {
    public class AdminController : BaseApiController
        {
        private readonly ITokenRepository tokenRepository;
        private readonly IPhotoRepository photoRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AdminController(ITokenRepository tokenRepository,IPhotoRepository photoRepository,UserManager<AppUser> userManager,IUnitOfWork unitOfWork,IMapper mapper)
            {
            this.tokenRepository = tokenRepository;
            this.photoRepository = photoRepository;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
            {
            var users = await userManager.Users
             .OrderBy(user => user.UserName
             
             )
             .Select(user => new
                 {
                 user.Id,
                 user.UserName ,
                 roles = user.appUserRoles.Select(appUserRoles => appUserRoles.Role.Name).ToList()
                 }).ToListAsync();
           return Ok(users);
            }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetStatistics")]
        public async Task<ActionResult<statisticsDto>> GetStatistics()
            {
            var totalUsers = await userManager.Users.CountAsync();
            var users = await userManager.Users.ToListAsync();

            if (users == null)
                return NotFound();

            int totalPatients = 0;
            int totalDoctors = 0;
            int totalClinics = 0;

            foreach (var user in users)
                {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Contains("Patient"))
                    totalPatients++;
                if (roles.Contains("Doctor"))
                    totalDoctors++;
                if (roles.Contains("Clinic"))
                    totalClinics++;
                }

            var statisticsDto = new statisticsDto
                {
                totalUser = totalUsers,
                totalPatient = totalPatients,
                totalDoctor = totalDoctors,
                totalClinic = totalClinics
                };

            return Ok(statisticsDto);
            }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username,[FromQuery]string roles )
            {
            if (string.IsNullOrEmpty(roles))
                return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(',').ToArray();
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));

            }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register( RegisterDto registerDto )
            {
            if (await UserExists(registerDto.username))
                return BadRequest("Username is Taken");
            var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.username.ToLower();
            var result = await userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var role = "";
            if (registerDto.gender == "Clinic")
                role = "Clinic";
            else
            if (registerDto.gender == "Doctor")
                role = "Doctor";
            else
                role = "Patient";

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                return BadRequest(result.Errors);

            return new UserDto
                {
                userName = user.UserName,
                Token = await tokenRepository.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                };
            }
        public async Task<bool> UserExists( string username )
            {
            return await userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
            }

        [Authorize(Policy ="ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
            {
            var photos = await unitOfWork.photoRepo.GetUnapprovedPhotos();
            return Ok(photos);
            }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto( int photoId )
            {
            var photo = await
    unitOfWork.photoRepo.GetPhotoById(photoId);
            if (photo == null)
                return NotFound("Could not find photo");
            photo.IsApproved = true;
            var user = await
           unitOfWork.UserRepository.GetUserByPhotoId(photoId);
            if (!user.Photos.Any(x => x.IsMain))
                photo.IsMain = true;
            await unitOfWork.Complate();
            return Ok();
            }
        [HttpDelete("delete-user/{username}")]
        public async Task<ActionResult> deleteUser(string username )
            {
            var user=  await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound("User is not found!");

            unitOfWork.UserRepository.DeleteUser(user.Id);
            if (await unitOfWork.Complate())
                return Ok();
            return BadRequest("Problem deleting user");
            }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
            {
            var photo = await unitOfWork.photoRepo.GetPhotoById(photoId);
            if (photo.PublicId != null)
                {
                var result = await photoRepository.DeletePhotoAsync(photo.PublicId);
                if (result.Result == "ok")
                    unitOfWork.photoRepo.RemovePhoto(photo);
                }
            else
                {
                 unitOfWork.photoRepo.RemovePhoto(photo);
                }
            return Ok();
            }
        }
    }
