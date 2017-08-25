using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace Educor.Api.AIBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        //internal static IDialog<ProfileForm> MakeRootDialog()
        //{
        //    return Chain.From(() => FormDialog.FromForm(ProfileForm.BuildProfileForm));
        //}

        //[ResponseType(typeof(void))]

        //public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        //{
        //    if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
        //    {
        //        await Conversation.SendAsync(activity, () =>
        //            { return Chain.From(() => FormDialog.FromForm(ProfileForm.BuildProfileForm)); });
        //    }

        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // Detect if this is a Message activity
            if (activity.Type == ActivityTypes.Message)
            {
                //// Get any saved values
                //StateClient sc = activity.GetStateClient();
                //BotData userData = sc.BotState.GetPrivateConversationData(
                //    activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                //var boolProfileComplete = userData.GetProperty<bool>("ProfileComplete");

                //if (boolProfileComplete)
                //{
                    // Call our FormFlow by calling MakeRootDialog

                    await Conversation.SendAsync(activity, () => new Dialogs.RootLuisDialog());
                //}
                //else
                //{
                    //// Get the saved profile values
                    //var firstName = userData.GetProperty<string>("FirstName");
                    //var lastName = userData.GetProperty<string>("LastName");
                    //var email = userData.GetProperty<string>("Email");

                    //// Tell the user their profile is complete
                    //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //sb.Append("Your profile is complete.\n\n");
                    //sb.Append($"FirstName = {firstName}\n\n");
                    //sb.Append($"LastName = {lastName}\n\n");
                    //sb.Append($"Email = {email}");

                    //// Create final reply
                    //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    //Activity replyMessage = activity.CreateReply(sb.ToString());
                    //await connector.Conversations.ReplyToActivityAsync(replyMessage);

                    //next message will be sent to LUIS
                    //await Conversation.SendAsync(activity, MakeRootDialog);

                }
        
                    ////        else
                    ////        {
                    ////             This was not a Message activity
                    ////            HandleSystemMessage(activity);
                    ////}

                    // Send response
             var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}