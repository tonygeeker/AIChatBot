using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Educor.Api.AIBot.Services;
using Educor.Api.Data.DbContexts;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Educor.Api.AIBot.Dialogs
{

    [LuisModel("d665f356-a7eb-46a5-9b79-1f1ad0b9820a","18f34caddf394c9a8ed3d96b454a7fca")]

    [Serializable]

    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("SearchCourse")]

        public async Task GetSearchResult(IDialogContext context, LuisResult result)
        {
            //foreach (var entity in result.Entities)
            //{
                //var entityValue = entity.Entity.ToLower();
                //EntityRecommendation rec;

                //if (entityValue.Contains("course") || entity.Type.ToLower().Contains("course"))
                //{
                    PromptDialog.Confirm(context, ResumeAfterConfirm, "You are querying about courses, correct?", null, 2,
                        PromptStyle.Auto);
               // }
            //}
            
        }

        private async Task ResumeAfterConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await context.PostAsync("Please Select option to help refine your query:");
                var searchOptions = new List<string>
                {
                    "Search by Course Category",
                    //"Search by Course Start Date",
                    //"Search by Course Name",
                    "Show All Courses on Offer",
                };

                var options = new PromptOptions<string>("Select how you would like to refine your query",
                        "Sorry, that's not one of the options. Please try again..", "You chose an incorrect option twice, " +
                        "I'm sorry you might have misunderstood...",
                        searchOptions, 2);

                PromptDialog.Choice<string>(context, ProcessSearchOption, options);
            }
            else
            {
                await context.PostAsync("Please rephrase your query. Type 'help' if you need help");
            }

        }

        private async Task ProcessSearchOption(IDialogContext context, IAwaitable<object> result)
        {
            var repo = new EducorDbRepo(null);
            var chosenOption = await result;
            var tenantId = 1;
            var courseSearchService = new CourseSearchService();
            var reply = context.MakeMessage();
            var pictureUrl = "";


            switch (chosenOption.ToString())
            {
                case "Search by Course Category":

                    reply.Attachments = new List<Attachment>();
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var courses = new List<Data.Models.Course>();
                    var courseCategories = CourseCategory.CreateList();

                    var categoryButtons = new CardAction()
                    {
                        Type = "openUrl",
                        Title = "View Details",
                        Value = "https://www.cityvarsityonline.com/courses/"
                    };

                    foreach (var courseCategory in courseCategories)
                    {
                        var thumbnail = new ThumbnailCard()
                        {
                            Title = courseCategory.Title,
                            Subtitle = courseCategory.SubTitle,
                            Buttons = new List<CardAction>()
                        };
                        thumbnail.Buttons.Add(categoryButtons);
                        thumbnail.Images.Add(new CardImage(courseCategory.PictureUrl));
                        reply.Attachments.Add(thumbnail.ToAttachment());
                    }
                    await context.PostAsync("Please Select the course category:");
                    await context.PostAsync("Each link will take you to the website courses");
                    await context.PostAsync(reply);
                    break;

                case "Search by Course Start Date":

                    var form = new FormDialog<StartDateForm>(
                        new StartDateForm(),
                        StartDateForm.BuildForm
                        );

                    context.Call<StartDateForm>(form, DateFormComplete);

                    break;

                case "Search by Course Name":

                    await context.PostAsync("Please name the course you are looking for:");
                    var courseName = await result;

                    courses = repo.CourseQueries.GetAllForTenant(tenantId)
                        .Where(x => x.IsVisible && x.Name.Contains(courseName.ToString())).ToList();

                    reply = context.MakeMessage();
                    reply.Attachments = new List<Attachment>();
                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    var namesButtons = new CardAction()
                    {
                        Type = "openUrl",
                        Title = "View Details",
                        Value = "https://www.cityvarsityonline.com/course-schedule"
                    };

                    pictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/12/Creative-Writing.png";

                    foreach (var course in courses)
                    {

                        var thumbnail = new ThumbnailCard()
                        {
                            Title = course.Name,
                            Subtitle = $"Start Date: {course.Semester.Substring(0, 8)}\n" +
                                       $"End Date: {course.Semester.Substring(12, 20)}",
                            Buttons = new List<CardAction>()
                        };
                        thumbnail.Buttons.Add(namesButtons);
                        thumbnail.Images.Add(new CardImage(pictureUrl));
                        reply.Attachments.Add(thumbnail.ToAttachment());
                    }

                    await context.PostAsync("Here are courses with similar or related names:");
                    await context.PostAsync(reply);
                    break;

                default:
                // ReSharper disable once RedundantCaseLabel
                case "Show All Courses on Offer":

                    //PromptDialog.Confirm(context, ViewListOfAllCourses, "Please confirm that want to view all courses?",
                    //    "Please click an option below or type 'yes' or 'no'", 2, PromptStyle.Auto, new string[] { "YES", "NO" });


                    tenantId = 1;

                    repo = new EducorDbRepo(null);
                    courses = repo.CourseQueries.GetAllForTenant(tenantId).Where(x => x.IsVisible).ToList();

                    var response = context.MakeMessage();
                    response.Attachments = new List<Attachment>();
                    response.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    var datesButtons = new CardAction()
                    {
                        Type = "openUrl",
                        Title = "View Details",
                        Value = "https://www.cityvarsityonline.com/course-schedule"
                    };

                    pictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/12/Creative-Writing.png";

                    foreach (var course in courses)
                    {

                        var thumbnail = new ThumbnailCard()
                        {
                            Title = course.Name,
                            Subtitle = $"Price: R{ course.OriginalCost}\n" +
                                       $"College: City Varsity Online\n" +
                                       $"Start Date: {course.Semester}\n \n",
                            Buttons = new List<CardAction>()
                        };
                        thumbnail.Buttons.Add(datesButtons);
                        thumbnail.Images.Add(new CardImage(pictureUrl));
                        response.Attachments.Add(thumbnail.ToAttachment());
                    }

                    await context.PostAsync("Please select course from our list below:");
                    await context.PostAsync(response);
                    context.Wait(MessageReceived);
                    break;
            }

        }

        private async Task ViewListOfAllCourses(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                var tenantId = 1;

                var repo = new EducorDbRepo(null);
                var courses = repo.CourseQueries.GetAllForTenant(tenantId).Where(x => x.IsVisible).ToList();

                var response = context.MakeMessage();
                response.Attachments = new List<Attachment>();
                response.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                var datesButtons = new CardAction()
                {
                    Type = "openUrl",
                    Title = "View Details",
                    Value = "https://www.cityvarsityonline.com/course-schedule"
                };

                var pictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/12/Creative-Writing.png";

                foreach (var course in courses)
                {

                    var thumbnail = new ThumbnailCard()
                    {
                        Title = course.Name,
                        Subtitle = $"StartDate: {course.Semester}",
                        Buttons = new List<CardAction>()
                    };
                    thumbnail.Buttons.Add(datesButtons);
                    thumbnail.Images.Add(new CardImage(pictureUrl));
                    response.Attachments.Add(thumbnail.ToAttachment());
                }

                await context.PostAsync("Please select course from our list below:");
                await context.PostAsync(response);
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("I'm sorry I must have misunderstood, please rephrase your query");
                context.Wait(MessageReceived);
            }
        }

        private async Task DateFormComplete(IDialogContext context, IAwaitable<StartDateForm> result)
        {
            var tenantId = 1;

            StartDateForm form = null;
            try
            {
                form = await result;
            }
            catch (OperationCanceledException)
            {
            }

            if (form == null)
            {
                await context.PostAsync("You cancelled the form.");
            }
            else
            {
                var month = form.Month;
                var startDate = month + form.Year;
                var courseSearchService = new CourseSearchService();
                var courses = courseSearchService.SearchByStartDate(startDate, tenantId,
                    form.BeforeDuringAfter.ToString());

                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                var datesButtons = new CardAction()
                {
                    Type = "openUrl",
                    Title = "View Details",
                    Value = "https://www.cityvarsityonline.com/course-schedule"
                };

                var pictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/12/Creative-Writing.png";

                foreach (var course in courses)
                {

                    var thumbnail = new ThumbnailCard()
                    {
                        Title = course.Name,
                        Subtitle = $"StartDate: {course.Semester}",
                        Buttons = new List<CardAction>()
                    };
                    thumbnail.Buttons.Add(datesButtons);
                    thumbnail.Images.Add(new CardImage(pictureUrl));
                    reply.Attachments.Add(thumbnail.ToAttachment());
                }

                await context.PostAsync("Please select course from below:");
                await context.PostAsync(reply);
            }

        }

        [LuisIntent("Bursaries")]

        public async Task GetBursaryResult(IDialogContext context, LuisResult result)
        {
            var message = "Sorry but we do not offer any bursaries for now, but we have very flexible and affordable payment options";
            
            await context.PostAsync(message);

            PromptDialog.Confirm(context, ResumeAfterPaymentConfirm, "Would you like to see our flexible payment plans or options?", null, 2,
                        PromptStyle.Auto);
        }

        public async Task ResumeAfterPaymentConfirm(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await context.PostAsync("Here are the various payment options that we have:");
                var paymentOptions = new List<string>
                {
                    "R2000 deposit, 3/6/10 monthly installments",
                    "Payment by credit card",
                    "Payment by cash",

                };

                var options = new PromptOptions<string>("Please specify which course are you interested and I will show you the actual price amounts",
                        "Sorry, that's not one of the options. Please try again..", "You chose an incorrect option twice, " +
                        "I'm sorry you might have misunderstood...",
                        paymentOptions, 2);

                PromptDialog.Choice<string>(context, ProcessPaymentOption, options);
            }
            else
            {
                await context.PostAsync("Ok would you like me to assist with anything else?");
            }
        }

        public async Task ProcessPaymentOption(IDialogContext context, IAwaitable<string> result)
        {
            switch (result.ToString())
            {
                case "R2000 deposit, 3/6/10 monthly installments":
                    await context.PostAsync("You can choose how many installments (3. 6 or 10 months) you want to pay but the deposit is R2000");
                    await context.PostAsync("E.g. if a course costs R6365, you pay R2000 depost upfront");
                    await context.PostAsync("Then you may choose the installment plan () that you'll use to settle the remainder of R4365");
                    break;
                case "Payment by credit card":
                    await context.PostAsync("You need to pay the full cost of the course");
                    break;
                default:
                case "Payment by cash":
                    await context.PostAsync("You need to pay the full cost of the course and make a cash deposit onto our bank account");
                    break;
            }
        }

        [LuisIntent("Accreditation")]

        public async Task GetAccreditationResult(IDialogContext context, LuisResult result)
        {
            var message = "";
            if (result.Query.ToLower().Contains("NQF"))
            {
                message = "Our online courses are not NQF accredited but are SABPP accredited, and are recognized by SAQA";
            }
            else if (result.Query.ToLower().Contains("SAQA"))
            {
                message = "Our courses are SABPP accredited and are recognized by SAQA";
                message += "SABPP is ......";
            }
            else if (result.Query.ToLower().Contains("SABPP"))
            {
                message = "All our courses are SABPP accredited and are recognized by SAQA";
                message += "SABPP as a professional programme and this can earn you CPD points.";
            }
            else
            {
                message = "Our online courses are SABPP accredited and are recognized by SAQA";
                message += "SABPP as a professional programme and it can earn you “Continuing Professional Development” (CPD) points. ";
            }

            await context.PostAsync(message);
        }

        [LuisIntent("InternationalQueries")]

        public async Task GetInternationalResult(IDialogContext context, LuisResult result)
        {
            var message = "As an international student, you will be able to register. However you will be expected to pay the full price upfront.";
            await context.PostAsync(message);
            message += "You may either pay via credit card or using cash ";
            await context.PostAsync(message);
        }

        [LuisIntent("PaymentOptions")]

        public async Task GetPaymentOptions(IDialogContext context, LuisResult result)
        {
            var message = "Looks like you want to find out about our payment option...";
            await context.PostAsync(message);
            PromptDialog.Confirm(context, ResumeAfterPaymentConfirm, "Would you like to see our flexible payment plans or options?", null, 2,
                        PromptStyle.Auto);
        }

        [LuisIntent("howOnlineWorks")]

        public async Task GetHowOnlineWorks(IDialogContext context, LuisResult result)
        {
            var message = "The online learning experience has been designed to match the increasing demand for"+
                " technologically skilled and creative professionals in industries.\nWith CityVarsity Online(CVO), " +
                          "students can now literally study anywhere, anytime and receive the support of professionals " +
                          "who are at the leading edge of their respective industries.\nEssentially you don’t attend any " +
                          "classes, all assessments are submitted online through our learner management system which you " +
                          "will have access to once enrolled.";
            await context.PostAsync(message);
        }

        [LuisIntent("Reassurance")]

        public async Task GetReassurance(IDialogContext context, LuisResult result)
        {
            var message = "City-Varsity is proud to bring you a new learning platform that has embraced and harnessed " +
                          "the power of educational technology.\n Building on our 20 year legacy of successfully offering " +
                          "creative full and part-time qualifications in Cape Town and Johannesburg, a new online education " +
                          "product has emerged.\n Prepare for the dynamic, creative and digitally diverse learning experience " +
                          "of City - Varsity Online.\n Students may now become immersed in their chosen field by studying creative " +
                          "courses designed by subjects matter experts and industry leaders";
            await context.PostAsync(message);
        }

        [LuisIntent("Enrolled")]

        public async Task GetEnrolled(IDialogContext context, LuisResult result)
        {
            var message = "";
            if ((result.Query.ToLower().Contains("password")) ||
                (result.Query.ToLower().Contains("log") && result.Query.ToLower().Contains("in")))
            {
                message = "You will be sent a link via email wich will allow you to reset your passord";
                
            }
            else if ((result.Query.ToLower().Contains("already") && (result.Query.ToLower().Contains("enrolled"))) ||
                     (result.Query.ToLower().Contains("I don\'t see my course ")) || (result.Query.ToLower().Contains("not under my enrollments")))
            {
                message = "You will access your course material and info on the day that your course commences";
            }
            else
            {
                message = "You will be sent a link via email wich will allow you to reset your passord";
            }
            await context.PostAsync(message);
        }


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message;
            var lowerCaseResult = result.Query.ToLower();

            if ((lowerCaseResult.Contains("hello")) || ((lowerCaseResult.Contains("hi")) || (lowerCaseResult.StartsWith("hey")) ||
                (lowerCaseResult.Contains("greetings") || lowerCaseResult.Contains("what's up") || lowerCaseResult.Contains("howzit")))) 
            {
                message = "Greetings, I'm Edubot the friendly bot. How may I assist you?...";
            }
            else if ((lowerCaseResult.Contains("thanks")) || (lowerCaseResult.Contains("thank you")))
            {
                message = "You are welcome, the pleasure is all mine...\n May I assist you with anything else?"
                    .Replace("\n", Environment.NewLine);
            }
            else if (lowerCaseResult.Contains("are you a bot") || lowerCaseResult.Contains("are you really a bot")
                ||lowerCaseResult.Contains("are you a real bot"))
            {
                message = "Yes, I'm Edubot I am a real bot! \n May I assist you with anything else?"
                    .Replace("\n", Environment.NewLine);
            }
            else if (lowerCaseResult.Contains("what can you do"))
            {
                message = "I can help you with our world class courses, ask me a question...";
            }
            else if (lowerCaseResult.Contains("do you speak english") || (lowerCaseResult.Contains("what language"))
                || lowerCaseResult.Contains("do you speak") || lowerCaseResult.Contains("what do you speak"))
            {
                message = "I can only speak in English for now. How may I assist you?";
            }
            else
            {
                message = $"Sorry, I did not understand '{result.Query}'.\n Please type 'help' if you need assistance."
                    .Replace("\n", Environment.NewLine);
            }

            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }


        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            var message = "I can help you with our world class courses, please select an option below...";

            var searchOptions = new List<string>{ "Query About Courses", "Registration Queries", "Show All Courses Offered"};

            var options = new PromptOptions<string>("Select which of these would you like to explore.",
                "Sorry please try again", "You have chosen incorrectly... I'm not sure I understand.", searchOptions, 2);


            await context.PostAsync("That's great! You've reached our help screen...");
            await context.PostAsync(message);

            PromptDialog.Choice<string>(context, ProcesHelpOptions, options);
        }

        private async Task ProcesHelpOptions(IDialogContext context, IAwaitable<string> result)
        {
            switch (await result)
            {
                case "Query About Courses":
                    PromptDialog.Confirm(context, ResumeAfterConfirm, "Please confirm that you are querying about courses?", 
                        "Please click an option below or type 'yes' or 'no'", 2, PromptStyle.Auto, new string[] {"YES", "NO"});

                    break;

                case "Registration Queries":
                    PromptDialog.Confirm(context, PromptNextEnrolmentOption, "Please confirm that you are querying about registration?",
                        "Please click an option below or type 'yes' or 'no'", 2, PromptStyle.Auto, new string[] { "YES", "NO" });

                    break;

                default:
                    //case "Show All Courses Offered":
                    PromptDialog.Confirm(context, ViewListOfAllCourses, "Please confirm that want to view all courses?",
                            "Please click an option below or type 'yes' or 'no'", 2, PromptStyle.Auto, new string[] { "YES", "NO" });

                    break;

            }
        }

        [LuisIntent("Enrol")]

        public async Task GetEnrolResult(IDialogContext context, LuisResult result)
        {
            //var course = new Course();

            //var message = "I can help you register with our world class courses, please select an option below...";

            PromptDialog.Confirm(context, PromptNextEnrolmentOption, "Please confirm that you are querying about registration?",
                         "Please click an option below or type 'yes' or 'no'", 2, PromptStyle.Auto, new string[] { "YES", "NO" });

        }

        private async Task PromptNextEnrolmentOption(IDialogContext context, IAwaitable<bool> result)
        {
            await context.PostAsync("You want to register? Please wait...");

            var searchOptions = new List<string>
            {
                "View Enrolment Process First",
                "Choose Course from Course List"
            };

            var options = new PromptOptions<string>("Please select which action would you like to do next",
                "Sorry please try again", "You have chosen incorrectly... I'm not sure I understand.", searchOptions, 2);

            PromptDialog.Choice<string>(context, ProcessNextEnrolmentOption, options);
        }


        private async Task ProcessNextEnrolmentOption(IDialogContext context, IAwaitable<string> result)
        {
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var courseCategories = CourseCategory.CreateList();

            var howToButtons = new CardAction()
            {
                Type = "openUrl",
                Title = "View Process",
                Value = "https://www.cityvarsityonline.com/how-it-works/"
            };

            var coursesButtons = new CardAction()
            {
                Type = "openUrl",
                Title = "Select Course",
                Value = "https://www.cityvarsityonline.com/courses/"
            };

            var howTothumbnail = new ThumbnailCard()
            {
                Title = "Enrolment Process",
                Subtitle = "This option will give you step by step guidance on how to apply and register to study with us.",
                Buttons = new List<CardAction>()
            };
            howTothumbnail.Buttons.Add(howToButtons);
            howTothumbnail.Images.Add(new CardImage("data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wC" +
                                                    "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"));

            var coursesThumbnail = new ThumbnailCard()
            {
                Title = "Choose Course to enrol",
                Subtitle = "This option will take you to the various courses on offer where you can pick one and register.",
                Buttons = new List<CardAction>()
            };
            coursesThumbnail.Buttons.Add(coursesButtons);
            coursesThumbnail.Images.Add(new CardImage("https://www.cityvarsityonline.com/wp-content/uploads/2016/10/SEO_WR-1.jpg"));

            reply.Attachments.Add(coursesThumbnail.ToAttachment());
            reply.Attachments.Add(howTothumbnail.ToAttachment());

            await context.PostAsync(reply);
            context.Wait(MessageReceived);

        }

     }
}
    

    



