using API.DTOs;
using API.Entities;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
    {
    public class FeedbackController : BaseApiController
        {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public FeedbackController( IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            }
        [HttpGet("{username}")]
        public async Task<ActionResult<FeedbackDto>> GetFeedbackAsync(string username)
            {
            var feedback = await unitOfWork.FeedbackRepository.GetByUserNameAsync(username);
            return Ok(mapper.Map<FeedbackDto>(feedback));
        }
        [HttpGet("{username}")]
        public async Task<ActionResult<FeedbackDto>> GetAllFeedbackUserNameAsync( string username )
            {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var feedback = await unitOfWork.FeedbackRepository.GetAllFeedbackByIdAsync(user.Id);
            return Ok(mapper.Map<FeedbackDto>(feedback));
            }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetAllFeedbackAsync()
            {
            var feedbacks = await unitOfWork.FeedbackRepository.GetAllAsync();
            if (feedbacks == null)
                return NotFound("Feedbacks is Not Found");
            return Ok(mapper.Map<IEnumerable<FeedbackDto>>(feedbacks));
            }
        [HttpPost("Add-Feedback")]
        public async Task<ActionResult<FeedbackDto>> PostFeedback( [FromBody] FeedbackDto feedbackDto )
            {
            var feedback = mapper.Map<Feedback>(feedbackDto);
            await unitOfWork.FeedbackRepository.AddAsync(feedback);

            if (await unitOfWork.Complate())
                {
                var createdFeedback = mapper.Map<FeedbackDto>(feedback);
                return Ok(createdFeedback);
                }

            return BadRequest("Failed to add feedback");
            }

        [HttpDelete("delete-Feedback/{id}")]
        public async Task<ActionResult<FeedbackDto>> DeleteFeedback( int id )
            {
             await unitOfWork.FeedbackRepository.DeleteAsync(id);
            if (await unitOfWork.Complate())
                return Ok();
            return BadRequest("Problem to Delete");
            }
        }
    }
