using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
    {
    public class PostController : BaseApiController
        {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PostController( IUnitOfWork unitOfWork, IMapper mapper )
            {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            }
        [HttpPost("add-post")]
        public async Task<ActionResult> AddPost( CreatePostDto createPostDto )
            {
            var post = mapper.Map<Post>(createPostDto);
            post.SenderUsername= User.GetUsername();
            await unitOfWork.postRepository.AddPost(post);
            if (await unitOfWork.Complate())
                return Ok();
            return BadRequest("Problem to Add Post");
            }
        [HttpDelete("delete-post/{id}")]
        public async Task<ActionResult> DeletePost( int id )
            {
            var result = await unitOfWork.postRepository.DeletePost(id);
            if (result)
                {
                if (await unitOfWork.Complate())
                    return Ok();
                }
            return BadRequest("Problem to Delete Post");
            }
        [HttpPut("update-post")]
        public async Task<ActionResult> UpdatePost( PostDto postDto )
            {
            var post = await unitOfWork.postRepository.GetPostById(postDto.Id);
            if (post == null)
                return NotFound();
            postDto.created = DateTime.UtcNow;
             mapper.Map(postDto, post);
            if (await unitOfWork.Complate())
                return Ok();
            return BadRequest("Failed to update user");
            }
        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPost()
            {
            var posts = await unitOfWork.postRepository.GetAllPosts();
            if (posts == null)
                return BadRequest("Posts not found");

            var postDtos = new List<PostDto>();
            foreach (var post in posts)
                {
                var postDto = mapper.Map<PostDto>(post);

                // Get the user associated with the post
                var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(post.SenderUsername);
                if (user != null)
                    {
                    // Get the photo associated with the user
                    var photo = await unitOfWork.photoRepo.getPhotoByAppUserId(user.Id);
                    if (photo != null && photo.Url != null)
                        {
                        postDto.url = photo.Url;
                        }
                    }

                postDtos.Add(postDto);
                }

            return Ok(postDtos);
            }

        [HttpGet("view-Posts/{username}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPostsBySenderUserName(string username)
            {
            var posts = await unitOfWork.postRepository.GetAllPostsBySenderUserName(username);
            if (posts == null)
                return BadRequest("posts is not found");
            var postmapper = mapper.Map<IEnumerable<PostDto>>(posts);
            return Ok(postmapper);
            }
        [HttpGet("user-Photo/{username}")]
        public async Task<ActionResult<string>> GetPhotoUser( string username )
            {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return BadRequest("User not found");

            var photo = await unitOfWork.photoRepo.getPhotoByAppUserId(user.Id);
            if (photo == null)
                return BadRequest("Photo not found");
            if (photo.Url==null)
                return BadRequest("Url is not found");
            return Ok(photo.Url);
            }
        }
}
