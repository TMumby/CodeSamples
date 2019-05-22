using System;
using System.Web.Mvc;
using TouristTalk.Models;
using TouristTalk.Validation;

namespace TouristTalk.Controllers
{
    /// <summary>
    /// Controller for all Conversation related screens
    /// </summary>
    public class ConversationController : Controller
    {
        /// <summary>
        /// Displays page of all user conversations. 
        /// </summary>
        /// <returns>screen of user conversations</returns>
        [AuthorizeUser]
        public ActionResult MyConversations()
        {
            try
            {
                //Gets all conversations for user and populates view model
                MyConversationsViewModel model = new MyConversationsViewModel();

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }

        /// <summary>
        /// Screen for adding (starting) conversation
        /// </summary>
        /// <returns>form for adding conversation</returns>
        [AuthorizeUser]
        public ActionResult Create()
        {
            try
            {
                //Get all Tour Agents that can be selected
                TourAgentModel tourAgent = new TourAgentModel();

                //Create View data
                CreateConversationViewModel model = new CreateConversationViewModel(tourAgent.AllAgents);

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }

        /// <summary>
        /// Action for recieving create conversation data
        /// </summary>
        /// <param name="model">data of conversation to be added</param>
        /// <returns>adds conversation then redirects to the created conversation</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public ActionResult Create(CreateConversationViewModel model)
        {
            try
            {
                TourAgentModel tourAgent = new TourAgentModel(model.TravelAgentID);

                if (tourAgent.ValidAgent)
                {
                    ConversationModel conversation = new ConversationModel(tourAgent, model.Title);
                    return RedirectToAction("Conversation", "Conversation", new { id = conversation.ID });
                }

                return RedirectToAction("LogOff", "Account");
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
           
        }

        /// <summary>
        /// For displaying a conversation and all its messages and facility to add message
        /// </summary>
        /// <param name="id">conversation id</param>
        /// <returns>conversation screen</returns>
        [AuthorizeUser]
        public ActionResult Conversation(int id)
        {
            try
            {
                //get conversation data
                ConversationModel conversation = new ConversationModel(id);

                //initiate view model data
                ConversationViewModel model = new ConversationViewModel(conversation);
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }

        /// <summary>
        /// For recivieving post of new message to add to conversation
        /// </summary>
        /// <param name="model">new message data</param>
        /// <returns>conversation screen with new message added</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser]
        public ActionResult Conversation(ConversationViewModel model)
        {
            try
            {
                //get conversation data
                ConversationModel conversation = new ConversationModel(model.ConversationID);
                
                if (!ModelState.IsValid)
                {
                    model.Messages = conversation.Messages;
                    return View(model);
                }

                //Add message to conversation
                conversation.AddMessage(model.NewMessageText, model.File);

                //initiate view model data
                model = new ConversationViewModel(conversation);

                return RedirectToAction("Conversation", "Conversation", model.ConversationID);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }                   
        }
    }
}