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
    public class ClinicController : BaseApiController
        {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<AppUser> userManager;

        public ClinicController(IUnitOfWork unitOfWork,IMapper mapper, UserManager<AppUser> userManager )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
            }
        [HttpPost("accept")]
        public async Task<ActionResult> acceptAppointment(AppointmentDto appointmentDto)
            {
            var clinic = await unitOfWork.UserRepository.GetUserByUsernameAsync(appointmentDto.clinicUsername);
            appointmentDto.appUserId = clinic.Id;
            if (clinic.UserName == appointmentDto.userName)
                return BadRequest("cannot Appointment Yourself");
            var Appointment = await unitOfWork.appointmentRepository.GetAppointmentByUsername(appointmentDto.userName); 
            if(Appointment != null)    
            return BadRequest("Appointment is already exist");

                var appointmentMaper = mapper.Map<Appointment>(appointmentDto);
                await unitOfWork.appointmentRepository.AddAsync(appointmentMaper);
            if (await unitOfWork.Complate())
                return NoContent();
            return BadRequest("Problem to make Accept");
            }

        /***************************************/
        [HttpGet("view-schedule/{clinicUserName}")]
        public async Task<ActionResult<CreateScheduleDto>> GetSchedulesByClinicUserName(string clinicUserName)
            {
            var schedules = await unitOfWork.scheduleRepository.GetSchedulesByClinicUserNameAsync(clinicUserName);
            if (schedules == null)
                return NotFound("schedules Not Found");
            var scheduleDto = mapper.Map<IEnumerable<CreateScheduleDto>>(schedules);
            return Ok(scheduleDto);
            }
        [HttpDelete("delete-schedule")]
        public async Task<ActionResult> deleteSchedule( [FromQuery] string userName, [FromQuery] string clinicUserName )
            {
            var schedule = await unitOfWork.scheduleRepository.GetScheduleByUserNameClinicUserName(userName, clinicUserName);
            if (schedule == null)
                return BadRequest("Schedule is already deleted");

            var remove = await unitOfWork.scheduleRepository.deleteSchedule(userName, clinicUserName);
            if (remove)
                {
                return Ok();
                }
            return BadRequest("Problem to Delete Schedule");
            }
        [HttpPost("add-schedule")]
        public async Task<ActionResult> AddSchedule(  CreateScheduleDto createScheduleDto )
            {
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(createScheduleDto.clinicUserName);
            if (user == null)
                return NotFound("User not found");

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Clinic"))
                return Forbid("Current User is not a Clinic");

            var Patient = await unitOfWork.scheduleRepository.GetScheduleByUserName(createScheduleDto.userName);

            var Scheddule = mapper.Map<Schedule>(createScheduleDto);
            await unitOfWork.scheduleRepository.AddAsync(Scheddule);
            if (await unitOfWork.Complate())
                return Ok();
            return BadRequest("Problem to Add Schedule");
            }
        /****************************************/

        [Authorize(Policy = "RequireClinic")]

        [HttpPost("approve-appointment/{appointmentId}")]
        public async Task<ActionResult> ApproveAppointment( int appointmentId )
            {
            var appointment = await
            unitOfWork.appointmentRepository.GetAppointmentById(appointmentId);
            if (appointment == null)
                return NotFound("Could not find appointment");
            appointment.IsApproved = true;  
            await unitOfWork.Complate();
            return Ok();
            }
        [Authorize(Policy = "RequireClinic")]
        [HttpPost("reject-appointment/{appointmentId}")]
        public async Task<ActionResult> RejectAppointment( int appointmentId )
            {
            var appointment = await unitOfWork.appointmentRepository.GetAppointmentById(appointmentId);
            if (appointment == null)
                return NotFound("Appointment Is Not Found");

                 var result = await unitOfWork.appointmentRepository.DeleteAsync(appointmentId);
            if (result)
                {
                if (await unitOfWork.Complate())
                    return Ok();
                }
            return BadRequest("Problem to Reject Appointment");
            }
        [HttpGet("Appointment-to-clinic/{clinicUsername}")]
        public async Task<ActionResult<AppointmentWithIdDto>> GetAppointmentsForModeration(string clinicUsername)
            {
            var clinic = await unitOfWork.UserRepository.GetUserByUsernameAsync(clinicUsername);
            var appointments = await unitOfWork.appointmentRepository.GetAllAppointment(clinic.Id);
            var appointmentsDto = mapper.Map<IEnumerable<AppointmentWithIdDto>>(appointments);
            return Ok(appointmentsDto);
            }

        }
    }
