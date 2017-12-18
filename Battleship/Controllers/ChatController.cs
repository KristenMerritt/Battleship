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

        /// <summary>
        /// Controller for the Chat table
        /// </summary>
        /// <param name="chatRepo"></param>
        /// <param name="playerRepo"></param>
        public ChatController(ChatRepo chatRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _chatRepo = chatRepo;
        }

        /// <summary>
        /// Gets all of the chat from the DB.
        /// GET: api/Chat
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetAllChat()
        {
            return Json(_chatRepo.GetAllChat());
        }

        /// <summary>
        /// Gets all of the chat after a certain ID.
        /// GET: api/Chat/{chatId}
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("{chatId}")]
        public JsonResult GetRecentChat(int chatId)
        {
            return Json(_chatRepo.GetRecentChat(chatId));
        }

        /// <summary>
        /// Adds a new chat to the DB.
        /// POST: api/Chat
        /// </summary>
        /// <param name="input"></param>
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