using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace Educor.Api.AIBot
{
    [Serializable]
    public class ProfileForm
    {
        // these are the fields that will hold the data
        // we will gather with the form
        [Prompt("What is your first name? {||}")]
        public string FirstName { get; set; }
        [Prompt("Please enter your last name? {||}")]
        public string LastName { get; set; }
        [Prompt("Please enter your Email? {||}")]
        public string Email { get; set; }

        [Prompt("Please provide your contact numbers? {||}")]
        public string Telephone { get; set; }

        [Prompt("How may I assist you? Select a type of query from below {||}")]
        public Service ServiceRequired { get; set; }
        // This method 'builds' the form 
        // This method will be called by code we will place
        // in the MakeRootDialog method of the MessagesControlller.cs file

        public static IForm<ProfileForm> BuildProfileForm()
        {
            return new FormBuilder<ProfileForm>().AddRemainingFields().Build();
        }
    }

    // This enum provides the possible values for the 
    // Gender property in the ProfileForm class
    // Notice we start the options at 1 

    [Serializable]
    public enum Service
    {
        Courses, OnlineStudying, ApplicationRequirements, InternationalStudents,
        PaymentOption, AlreadyRegistered, BursaryInfo, Accreditation, Other
    };
}