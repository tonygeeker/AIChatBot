using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chronic;
using Chronic.Tags.Repeaters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace Educor.Api.AIBot
{
    public class StartDateForm
    {
        // these are the fields that will hold the data
        // we will gather with the form
        [Prompt("Please enter the month of interest? {||}")]
        public MonthName Month;
        [Prompt("Which year is this for {||}")]
        public string Year;
        [Template(TemplateUsage.EnumSelectOne, "Would you like to see course offered 'during', 'before' or 'after' specified date{||}",
                   "Please choose to view courses offered 'before', 'during' or 'after' {||}")]
        public Time BeforeDuringAfter { get; set; }
        // This method 'builds' the form 
        // This method will be called by code we will place
        // in the MakeRootDialog method of the MessagesControlller.cs file

        public static IForm<StartDateForm> BuildForm()
        {
            return new FormBuilder<StartDateForm>()
                    .Message("Allow me to gather some information about your query...")
                    .Field(nameof(Month))
                    .Field(nameof(Year))
                    .Field(nameof(BeforeDuringAfter))
                    .Build();
        }
    }

    [Serializable]
    public enum Time
    {
        IGNORE,
        [Terms("prior to", "earlier", "previous", "formerly", "in advance", "ring me", "up to", "sooner", "afore", "before that", "past")]
        Before,
        [Terms("at the time", "amid", "throughout", "at the same time", "at the time", "over", "midst", "in the midst of")]
        During,
        [Terms("subsequesntly", "later", "afterwards", "thereafter", "succeeding", "after that", "following", "future")]
        After
    };
}