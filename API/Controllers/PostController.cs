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
        private readonly UnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PostController( UnitOfWork unitOfWork, IMapper mapper )
            {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            }
        [HttpPost("add-post")]
        public async Task<ActionResult> AddPost( CreatePostDto createPostDto )
            {
            var post = mapper.Map<Post>(createPostDto);
            await unitOfWork.postRepository.AddPost(post);
            if (await unitOfWork.Complate())
                return Ok("Post Is Added");
            return BadRequest("Problem to Add Post");
            }
        [HttpDelete("delete-post/{id}")]
        public async Task<ActionResult> DeletePost( int id )
            {
            var result = await unitOfWork.postRepository.DeletePost(id);
            if (result)
                {
                if (await unitOfWork.Complate())
                    return Ok("Post Is Deleted");
                }
            return BadRequest("Problem to Delete Post");
            }
        [HttpPut("update-post")]
        public async Task<ActionResult> UpdatePost( PostDto postDto )
            {
            var post = await unitOfWork.postRepository.GetPostById(postDto.Id);
            if (post == null)
                return NotFound();
            mapper.Map(postDto, post);
            if (await unitOfWork.Complate())
                return Ok("Update is Confirm");
            return BadRequest("Failed to update user");
            }
        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetAllPost()
            {
            var posts = await unitOfWork.postRepository.GetAllPosts();
            if (posts == null)
                return BadRequest("posts is not found");
            var postmapper = mapper.Map<IEnumerable<PostDto>>(posts);
            return Ok(postmapper);
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
        }
}
