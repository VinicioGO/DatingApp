using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userReoisitory;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userReoisitory, IMapper mapper)
        {
            _mapper = mapper;
            _userReoisitory = userReoisitory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userReoisitory.GetMmbersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userReoisitory.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userReoisitory.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userReoisitory.Update(user);

            if (await _userReoisitory.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}