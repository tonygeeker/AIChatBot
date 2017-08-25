using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Educor.Api.AIBot
{
    public class CourseCategory
    {
        public string Title { get; set; }  

        public string[] Examples { get; set; }

        public string SubTitle { get; set; }

        public string HtmlUrl { get; set; }

        public string PictureUrl { get; set; }

        public static List<CourseCategory> CreateList()
        {
            var catList = new List<CourseCategory>();
            var cat1 = new CourseCategory()
            {
                Title = "Film",
                Examples = new string[]{ "Digital Cinematography", "Photography", "Journalism"},
                SubTitle = "From Script to Screen,become a professional film artist", 
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/09/Documentary_Film_Making_WR.jpg",
            };

            catList.Add(cat1);

            var cat2 = new CourseCategory()
            {
                Title = "Design",
                Examples = new string[] { "Interior Design", "Photography", "Journalism" },
                SubTitle = "From Script to Screen,become a professional film artist",
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2017/04/interior_design.jpeg",
            };

            catList.Add(cat2);

            var cat3 = new CourseCategory()
            {
                Title = "Journalism",
                Examples = new string[] { "Creative Writing", "Screenwriting", "Journalism" },
                SubTitle = "From Script to Screen,become a professional film artist",
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2017/04/intermediate_creative_writing.jpeg",
            };

            catList.Add(cat3);

            var cat4 = new CourseCategory()
            {
                Title = "Digital Design",
                Examples = new string[] { "Web Design", "Mobile App Design", "Social Media" },
                SubTitle = "From Script to Screen,become a professional film artist",
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/04/portfolio-item-01.jpg",  
            };

            catList.Add(cat4);

            var cat5 = new CourseCategory()
            {
                Title = "Sound",
                Examples = new string[] { "DJing", "Sound Business", "Etc" },
                SubTitle = "From Script to Screen,become a professional film artist",
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://www.cityvarsityonline.com/wp-content/uploads/2016/09/Djing_WR.jpg",
            };

            catList.Add(cat5);

            var cat6 = new CourseCategory()
            {
                Title = "Adobe",
                Examples = new string[] { "Illustrator", "Photoshop", "InDesign" },
                SubTitle = "From Script to Screen,become a professional film artist",
                HtmlUrl = "https://www.cityvarsityonline.com/courses/",
                PictureUrl = "https://cityvarsityonline.co.za/wp-content/uploads/2017/06/Illustrator32.jpg",
            };

            catList.Add(cat6);

            return catList;
        }
    }
}