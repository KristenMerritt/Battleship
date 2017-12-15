using System;
using Battleship.Cl;
using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Chat")]
    public class ChatController : BaseController
    {
        private readonly ChatRepo _chatRepo;
        private readonly PlayerRepo _playerRepo;

        public ChatController(ChatRepo chatRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _chatRepo = chatRepo;
        }

        //GET: api/Chat
        [HttpGet]
        public JsonResult GetAllChat()
        {
            return Json(_chatRepo.GetAllChat());
        }

        //GET: api/Chat/5
        [HttpGet]
        [Route("{chatId}")]
        public JsonResult GetRecentChat(int chatId)
        {
            return Json(_chatRepo.GetRecentChat(chatId));
        }

        //POST: api/Chat
        [HttpPost]
        public void AddNewChat(ChatMessageInfo input)
        {
            // validate token
            if (!base.ValidateToken(input.Token)) return;
            var tokenHelper = new Token();
            var userId = Int32.Parse(tokenHelper.DecodeToken(input.Token).GetValue("id").ToString());
            var chat = new db_Chat
            {
                Message = input.Message,
                Player_Id = userId
            };
            _chatRepo.AddNewChat(chat);
        }

    }
}