using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TouristTalk.Models
{
    /// <summary>
    /// Used to hold display data items for Creat Conversation View
    /// Acts as conduit between Create conversation view and conversation model.
    /// </summary>
    public class CreateConversationViewModel
    {
        [ScaffoldColumn(false)]
        [Display(Name = "Travel Agent")]
        public int TravelAgentID { get; set; }

        [ScaffoldColumn(false)]
        public List<SelectListItem> Names { get; private set; }
        
        public string Title { get; set; }

        //Initialiser can not be deleted as used in binding model to view
        public CreateConversationViewModel() {}

        /// <summary>
        /// Initilialises class - gets Tour Agents for select list
        /// </summary>
        /// <param name="tourAgents"></param>
        public CreateConversationViewModel(Dictionary<int, string> tourAgents)
        {
            Names = new List<SelectListItem>();
            foreach (KeyValuePair<int, string> tourAgent in tourAgents)
            {                
                Names.Add(new SelectListItem { Text = tourAgent.Value, Value = tourAgent.Key.ToString() });
            }
        }
    }
}